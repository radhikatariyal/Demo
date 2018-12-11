using Patient.Demographics.Common;
using Patient.Demographics.CrossCutting.Logger;
using Patient.Demographics.Commands;
using Patient.Demographics.Repository.Dtos;
using Patient.Demographics.Service.FileUploads.Importers;
using Patient.Demographics.Service.FileUploads.Validators.ValidationChecks;
using Patient.Demographics.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Patient.Demographics.Common.Csv;
using Patient.Demographics.Common.Extensions;
using Patient.Demographics.Service.Common;
using Patient.Demographics.Service.Exceptions;
using Patient.Demographics.Data.Entities.BatchProcess;
using Patient.Demographics.Common.Validation;
using Patient.Demographics.Domain;
using Patient.Demographics.Infrastructure.Mappers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Patient.Demographics.Repository.Repositories;
using Patient.Demographics.Repositories.Repositories;
using Patient.Demographics.Repositories.Dtos;
using static Patient.Demographics.Common.Enums;
using System.IO;
using System.Xml;
using Microsoft.VisualBasic.FileIO;

namespace Patient.Demographics.Service.FileUploads.Validators
{
    public abstract class UploadValidatorBase : UploadValidatorCore
    {
        private readonly IUploadsNotifier _uploadsNotifier;
        private readonly ILogger _logger;
        private readonly ISqlDataContextFactory _dataContextFactory;
        private int _numberOfErrors;
        private int _recordsToAdd;
        private int _recordsToUpdate;
        private const int ChunkSize = 1000;
        private readonly ISqlDataContext _dbContext;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISupplierRepository _supplierRepository;

        protected UploadValidatorBase(ICommandExecutor commandExecutor,
            IUploadsNotifier uploadsNotifier,
            ILogger logger,
            ISqlDataContextFactory dataContextFactory,
            ISqlDataContext dbContext,
            ICategoryRepository categoryRepository,
            ISupplierRepository supplierRepository
            ) : base(commandExecutor)
        {
            _uploadsNotifier = uploadsNotifier;
            _logger = logger;
            _dataContextFactory = dataContextFactory;
            _dbContext = dbContext;
            _categoryRepository = categoryRepository;
            _supplierRepository = supplierRepository;
        }

        protected abstract Task<IReadOnlyCollection<string>> GetExistingExternalIdentifiers(BatchProcessDto upload);
        protected abstract Task<BatchProcessMasterDto> GetMasterDto(BatchProcessDto upload);

        protected abstract Task<IList<BatchProductRulesDto>> GetRules(BatchProcessDto upload);


        protected virtual async Task<IList<IRowValidationCheck>> GetRowValidationChecks()
        {
            return await Task.FromResult(new List<IRowValidationCheck>());
        }

        public async Task ValidateAsync(BatchProcessDto upload, Guid updatedByUserId)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            _numberOfErrors = 0;

            if (upload == null)
            {
                throw new ApplicationException("Upload not found");
            }

            var dataContext = _dataContextFactory.Create();

            try
            {
                var errors = new Dictionary<string, ErrorCodes>();
                var completeFileName = ConfigurationManager.AppSettings["UploadedFilePath"] + "\\" + upload.BatchProcessType.Name + "\\" + upload.FileName;

                var status = await InsertDataToBatchProcessRecords(upload, completeFileName);

                var columnHeadings = await ParseColumnHeadings(dataContext, upload.Id);

                var numberOfDataRows = await dataContext.UploadRows.CountAsync(row => row.BatchProcessId == upload.Id && row.Status == BatchProcessRecordStatuses.NotValidated.Name);
                var rules = await GetRules(upload);
                List<string> columnNameCheck;

                if (upload.BatchProcessType.Name == "FltProduct")
                {
                    //if (completeFileName.ToLower().EndsWith(".xml"))
                    //{
                    //    foreach (var r in rules)
                    //    {
                    //        var s = r.SupplierFieldName.ToString().Replace(" ", string.Empty);
                    //        var i = columnHeadings.FindIndex(x => x.ToString() == s);
                    //        columnHeadings[i] = r.SupplierFieldName.ToString();
                    //    }
                    //}
                    columnNameCheck = rules.Select(f => f.SupplierFieldName).Except(columnHeadings).ToList();
                }
                else
                {
                    columnNameCheck = rules.Select(f => f.FieldName).Except(columnHeadings).ToList();
                }

                if (rules.Count != columnHeadings.Count)
                {
                    errors.Add($"{upload.BatchProcessType} rule contains {rules.Count} columns, upload file contains {columnHeadings.Count} columns", ErrorCodes.RowNumberOfColumnsNotCorrect);
                    _numberOfErrors++;
                }
                else if (columnNameCheck.Count != 0)
                {
                    errors.Add($"Unrecognised column name(s) {string.Join(", ", columnNameCheck)}", ErrorCodes.UnrecognisedColumnName);
                    _numberOfErrors++;
                }

                if (errors.Any())
                {
                    var joinedErrors = string.Join("; ", errors.Keys.ToList());
                    string str = string.Empty;

                    foreach (var er in errors)
                    {
                        int val = (int)er.Value;
                        str = str + er.Key + '-' + val + ',';
                    }
                    str = str.TrimEnd(',');
                    joinedErrors = str;
                    str = string.Empty;
                    foreach (ErrorCodes er in errors.Values)
                    {
                        int val = (int)er;
                        str = str + val + ",";
                    }
                    str = str.TrimEnd(',');
                    string ErrorCode = str;

                    await UpdateUploadWithErrorsAsync(upload.Id, joinedErrors, ErrorCode, updatedByUserId);
                    foreach (KeyValuePair<string, ErrorCodes> error in errors)
                    {
                        if (error.Value == ErrorCodes.RowNumberOfColumnsNotCorrect)
                            throw new UploadValidationException(ErrorCodes.RowNumberOfColumnsNotCorrect.ToString());
                        if (error.Value == ErrorCodes.UnrecognisedColumnName)
                            throw new UploadValidationException(ErrorCodes.UnrecognisedColumnName.ToString());
                    }



                }

                dataContext.DisableAutoDetectChanges();

                var numberOfChunks = numberOfDataRows / ChunkSize + 1;
                var rowsProcessed = 0;

                var notifierRate = NotificationUtilities.CalculateNotifierRate(numberOfDataRows);

                _recordsToAdd = 0;
                _recordsToUpdate = 0;

                DataTable successTable = new DataTable();

                foreach (var heading in columnHeadings)
                {
                    successTable.Columns.Add(heading.Replace(" ", ""));
                }

                successTable.Columns.Add("Succes/ErrorMessage");

                //SupplierInfoDto supplierInfo;
                CategoryPeopleSoftDto biwCategories = null;
                if (upload.BatchProcessType.Name == "FltCategory")
                {
                    //supplierInfo = await _supplierRepository.SupplierInfoBySupplierUserId(updatedByUserId);
                    var supplierInfo = await _supplierRepository.GetSupplierByIdAsync(upload.SupplierId);
                    biwCategories = _categoryRepository.GetCategoriesPeopleSoft(supplierInfo.PeopleSoftSetId);
                }

                var errorTable = successTable.Clone();

                for (var chunk = 0; chunk < numberOfChunks; chunk++)
                {
                    var rows = await GetChunk(dataContext, upload.Id, ChunkSize);

                    foreach (var row in rows)
                    {
                        if (_numberOfErrors >= 100) { break; }
                        errors = new Dictionary<string, ErrorCodes>();
                        var dataRowSuccess = successTable.NewRow();
                        var dataRowError = errorTable.NewRow();

                        var numberOfColumns = columnHeadings.Count;

                        var items = ParseJsonRow(row);

                        if (items.Count == 0)
                        {
                            errors.Add("Invalid row. The row contains no data.", ErrorCodes.RowContainsNoData);
                            _numberOfErrors++;
                        }
                        else if (items.Count != numberOfColumns)
                        {
                            errors.Add($"Line specification defines {numberOfColumns} columns however the current line has {items.Count} columns.", ErrorCodes.NumberOfColumnsNotEqual);
                            _numberOfErrors++;
                        }
                        else
                        {
                            for (int index = 0; index < columnHeadings.Count; index++)
                            {
                                dataRowSuccess[index] = Convert.ToString(items[index]);
                                dataRowError[index] = Convert.ToString(items[index]);

                                if (rules[index].Required && items[index].Length == 0)
                                {
                                    errors.Add($"{columnHeadings[index]} is missing", ErrorCodes.RowContainsNoData);
                                    _numberOfErrors++;
                                }

                                if (rules[index].Required && items[index].Length > 0)
                                {
                                    if (items[index].Length > rules[index].Max)
                                    {

                                        errors.Add($"Maximum allowed characters for {columnHeadings[index]} is {rules[index].Max}", ErrorCodes.MismatchMaximumAllowedCharacters);
                                        _numberOfErrors++;
                                    }

                                    if (items[index].Length < rules[index].Min)
                                    {
                                        errors.Add($"Minimum {rules[index].Min} characters required for for {columnHeadings[index]}", ErrorCodes.MismatchMininumAllowedCharacters);
                                        _numberOfErrors++;
                                    }
                                }

                                if (rules[index].FieldType == DataTypes.Int.Name && !DataValidation.IsInt(items[index]))
                                {
                                    errors.Add($"{columnHeadings[index]} is invalid", ErrorCodes.DataTypesInvalid);
                                    _numberOfErrors++;
                                }

                                //if (rules[index].FieldType == DataTypes.ImageUrl.Name && !DataValidation.IsImagePresentAtUrl(items[index]))
                                //{
                                //    errors.Add($"{columnHeadings[index]} is not a valid image");
                                //    _numberOfErrors++;
                                //}
                            }
                            BatchProcessSummaryDto result = null;
                            if (upload.BatchProcessType.Name == "FltCategory")
                            {
                                result = await ValidateBusinessRule(dataContext, upload, items, columnHeadings, errors, updatedByUserId, biwCategories.CategoryList);
                            }
                            else
                            {
                                result = await ValidateBusinessRule(dataContext, upload, items, columnHeadings, errors, updatedByUserId, null);
                            }

                            if (result.Errors.Any())
                            {
                                string str = string.Empty;

                                foreach (var er in errors)
                                {
                                    int val = (int)er.Value;
                                    str = str + er.Key + '-' + val + ',';
                                }
                                str = str.TrimEnd(',');
                                row.Errors = str;
                                str = string.Empty;
                                foreach (ErrorCodes er in errors.Values)
                                {
                                    int val = (int)er;
                                    str = str + val + ",";
                                }
                                str = str.TrimEnd(',');
                                row.ErrorCode = str;
                                row.Status = BatchProcessRecordStatuses.Error.Name;
                                // _numberOfErrors++;

                                dataRowError[columnHeadings.Count] = row.Errors;

                                errorTable.Rows.Add(dataRowError);
                            }
                            else
                            {
                                if (result.RecordsToAdd == 1)
                                {
                                    _recordsToAdd++;
                                    dataRowSuccess[columnHeadings.Count] = "Inserted";
                                }
                                else if (result.RecordsToUpdate == 1)
                                {
                                    _recordsToUpdate++;
                                    dataRowSuccess[columnHeadings.Count] = "Updated";
                                }

                                row.Status = BatchProcessRecordStatuses.Ok.Name;
                                successTable.Rows.Add(dataRowSuccess);
                            }

                            dataContext.SetModified(row);
                        }

                        if (rowsProcessed % notifierRate == 0)
                        {
                            int percentage = (int)Math.Round((double)(100 * rowsProcessed) / numberOfDataRows);

                            if (percentage < 7) percentage = 7;

                            await OutputStatusUpdate(upload.Id, rowsProcessed, numberOfDataRows, "Validation in progress : " + upload.FileName, percentage);
                        }

                        rowsProcessed++;
                    }

                    dataContext.DetectChanges();
                    await dataContext.SaveChangesAsync();

                    _dataContextFactory.Release(dataContext);

                    dataContext = _dataContextFactory.Create();
                    dataContext.DisableAutoDetectChanges();
                }

                var logFileName = completeFileName.Remove(completeFileName.Length - 5) + "_Log.xlsx";

                ExcelUtility.WriteExcelSuccessErrorSheet(logFileName, successTable, errorTable);


                await OutputStatusUpdate(upload.Id, numberOfDataRows, numberOfDataRows, "Validation completed", 100);

                await SetCounts(upload, updatedByUserId, _recordsToAdd, _recordsToUpdate, numberOfDataRows);

                dataContext.DetectChanges();

                await dataContext.SaveChangesAsync();
                if (_numberOfErrors >= 1 && _numberOfErrors <= 99)
                {
                    throw new UploadValidationException(BatchProcessRecordStatuses.FileUploadedwithFormattingErrors.Name);
                }
                if (_numberOfErrors >= 100)
                {
                    throw new UploadValidationException(BatchProcessRecordStatuses.ErrorFileNotUploaded.Name);
                }

                _logger?.LogInfo("Validation Finished");
            }
            finally
            {
                _dataContextFactory.Release(dataContext);
            }

            stopwatch.Stop();
            _logger?.LogInfo($"validation of upload {upload.Id} took {stopwatch.ElapsedMilliseconds}ms");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="upload"></param>
        /// <param name="completeFileName"></param>
        /// <returns></returns>
        private async Task<bool> InsertDataToBatchProcessRecords(BatchProcessDto upload, string completeFileName)
        {
            List<UploadRow> uploadRows = new List<UploadRow>();
            int rowNumber = 1;
            if (completeFileName.ToLower().EndsWith(".xml"))
            {
                //XmlTextReader xmlReader = new XmlTextReader(completeFileName);           
               
                //var header = new List<string>();
                //var txt = new List<string>();
                //while (xmlReader.Read())
                //{

                //    switch (xmlReader.NodeType)
                //    {
                //        case XmlNodeType.Element:
                //            if (rowNumber == 1)
                //            {
                //                if (xmlReader.Name == "product" || xmlReader.Name == "products") continue;
                //                header.Add(xmlReader.Name);
                //            }
                //            break;
                //        case XmlNodeType.Text:
                //            txt.Add(xmlReader.Value);
                //            break;
                //        case XmlNodeType.EndElement:
                //            // listBox1.Items.Add("");
                //            if (xmlReader.Name == "product")
                //            {
                //                if (rowNumber == 1)
                //                {
                //                    var Headerjson = JsonConvert.SerializeObject(header);
                //                    var headeruploadRow = UploadRow.CreateNew(upload.Id, rowNumber, Headerjson, upload.ProductFileId, null);
                //                    uploadRows.Add(headeruploadRow);
                //                    rowNumber++;
                //                }
                //                var json = JsonConvert.SerializeObject(txt);
                //                UploadRow uploadRow = UploadRow.CreateNew(upload.Id, rowNumber, json, upload.ProductFileId, null);
                //                uploadRows.Add(uploadRow);
                //                rowNumber++;

                //            }
                //            break;
                //    }

                //}

            }
           else if (completeFileName.ToLower().EndsWith(".txt"))
            {
                FileInfo file = new FileInfo(completeFileName);
                string text = File.ReadAllText(file.FullName);
                string[] lines = File.ReadAllLines(file.FullName);
                foreach (string line in lines)
                {

                    TextFieldParser parser = new TextFieldParser(new StringReader(line));
                    parser.HasFieldsEnclosedInQuotes = true;
                    parser.SetDelimiters("|");
                    string[] fields;
                    while (!parser.EndOfData)
                    {
                        fields = parser.ReadFields();                   
                        var json = JsonConvert.SerializeObject(fields);
                        UploadRow uploadRow = UploadRow.CreateNew(upload.Id, rowNumber, json, upload.ProductFileId, null);
                        uploadRows.Add(uploadRow);
                        rowNumber++;
                    }
                    parser.Close();
                }

            }
            else
            {
                var olddbReader = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=NO;IMEX=1;ImportMixedTypes=Text'", completeFileName);
                using (OleDbConnection excel_con = new OleDbConnection(olddbReader))
                {
                    excel_con.Open();
                    string sheet1 = "";
                    DataTable Sheets = excel_con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (upload.BatchProcessType.Name == "FltProduct")
                        sheet1 = Sheets.Rows[1]["Table_Name"].ToString();
                    else
                        sheet1 = Sheets.Rows[0]["Table_Name"].ToString();

                    OleDbCommand command = new OleDbCommand("SELECT * FROM [" + sheet1 + "]", excel_con);
                    using (OleDbDataReader dr = command.ExecuteReader())
                    {

                        while (dr.Read())
                        {
                            var row = dr[0];
                            var csv = new List<string>();
                            for (int i = 0; i < dr.FieldCount; i++)
                            {
                                row = row + "," + dr[i];
                                csv.Add(Convert.ToString(dr[i]));
                            }

                            var json = JsonConvert.SerializeObject(csv);
                            UploadRow uploadRow = UploadRow.CreateNew(upload.Id, rowNumber, json, upload.ProductFileId, null);
                            uploadRows.Add(uploadRow);
                            rowNumber++;
                        }

                    }
                }
            }

            var uploadRowEntities = uploadRows.Select(AggregateMapper.Map);
            _dbContext.BulkInsert(uploadRowEntities);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        private async Task<IList<string>> ParseColumnHeadings(ISqlDataContext dataContext, Guid uploadId)
        {
            var columnHeadingRow = await dataContext.UploadRows.SingleAsync(row => row.BatchProcessId == uploadId && row.Status == BatchProcessRecordStatuses.Header.Name);
            return ParseJsonRow(columnHeadingRow);
        }

        private IList<string> ParseCsvRow(BatchProcessRecordEntity row)
        {
            using (var parser = CsvHelperFactory.CreateParser(row.Row))
            {
                return parser.GetTrimmedValues();
            }
        }

        private IList<string> ParseJsonRow(BatchProcessRecordEntity row)
        {
            return JArray.Parse(row.Row).ToObject<string[]>();
        }

        private async Task<IList<BatchProcessRecordEntity>> GetChunk(ISqlDataContext dataContext, Guid uploadId, int chunkSize)
        {
            var chunk = await dataContext.UploadRows.Where(row => row.BatchProcessId == uploadId && row.Status == BatchProcessRecordStatuses.NotValidated.Name).OrderBy(row => row.RowNumber).Take(chunkSize).ToListAsync();
            return chunk;
        }

        protected abstract Task<Dictionary<ColumnChecker, int>> GetIndexedColumnCheckers(BatchProcessDto upload, IList<LiteralColumnMappingDto> columnMappings, IList<HierarchyColumnMappingDto> hierarchyColumnMappings, string externalIdentifierColumnName, Guid updatedByUserId, IList<string> columnHeadings);

        protected abstract Task<BatchProcessSummaryDto> ValidateBusinessRule(ISqlDataContext dataContext, BatchProcessDto upload, IList<string> items, IList<string> columnHeaders, Dictionary<string, ErrorCodes> errors, Guid updatedByUserId, object obj);
        // protected abstract Task<BatchProcessSummaryDto> ValidateBusinessRule(ISqlDataContext dataContext, BatchProcessDto upload, IList<string> items, IList<string> columnHeaders, IList<string> errors, Guid updatedByUserId);

        private static void PerformRowChecks(List<string> errors, IList<IRowValidationCheck> rowValidationChecks, IList<string> destinationColumnNames, IList<string> items)
        {
            var labelledFields = new FieldCollection();
            for (int i = 0; i < destinationColumnNames.Count; i++)
            {
                if (destinationColumnNames[i] != FieldCollection.IgnoredColumnName)
                {
                    labelledFields.AddField(destinationColumnNames[i], items[i]);
                }
            }

            foreach (var rowValidationCheck in rowValidationChecks)
            {
                rowValidationCheck.PerformCheck(errors, labelledFields);
            }
        }

        private async Task SetCounts(BatchProcessDto upload, Guid updatedByUserId, int numberToAdd, int numberToUpdate, int numberOfDataRows)
        {
            await UpdateUploadWithValidationResultsAsync(upload.Id, _numberOfErrors, numberToAdd, 0, numberToUpdate, numberOfDataRows, updatedByUserId);
            _logger?.LogInfo(" _numberOfErrors : " + _numberOfErrors + ", numberToAdd : " + numberToAdd + ", numberToUpdate : " + numberToUpdate);
        }

        private async Task OutputStatusUpdate(Guid uploadId, int rowsProcessed, int totalRows, string message, int percentage)
        {
            DebugConsole.WriteLine($"Sending Validation Status Update {rowsProcessed}/{totalRows}");
            await _uploadsNotifier.Notify(uploadId, BatchProcessStatuses.ValidationInProgress, rowsProcessed, totalRows, message, percentage);
        }

        protected int GetColumnIndex(IList<string> columns, ColumnMappingBaseDto mapping)
        {
            return columns.FindIndex(c => c.Equals(mapping.SourceColumn, StringComparison.CurrentCultureIgnoreCase));
        }

        protected void AddCommonValidators(string externalIdentifierColumnName, LiteralColumnMappingDto simpleColumnMapping, ColumnChecker columnChecker)
        {
            if (simpleColumnMapping.DataType == DataTypes.Text)
            {
                columnChecker.AddValidator(new TextCheck());
            }
            else if (simpleColumnMapping.DataType == DataTypes.Date)
            {
                columnChecker.AddValidator(new DateCheck());
            }
            else if (simpleColumnMapping.DataType == DataTypes.Int)
            {
                columnChecker.AddValidator(new IntegerCheck());
            }
            else if (simpleColumnMapping.DataType == DataTypes.Decimal)
            {
                columnChecker.AddValidator(new DecimalCheck());
            }
            if (simpleColumnMapping.Destination == externalIdentifierColumnName)
            {
                columnChecker.AddDuplicateExternalIdentityChecker(new DuplicateExternalIdentifierCheck());
            }
        }
    }
}