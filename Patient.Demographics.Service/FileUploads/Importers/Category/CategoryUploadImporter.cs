using Patient.Demographics.Common;
using Patient.Demographics.Repository.Repositories;
using Patient.Demographics.Service.FileUploads.Importers.User;
using Patient.Demographics.Data;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Patient.Demographics.Repository.Dtos;
using System.Collections.Generic;
using System.Diagnostics;
using Patient.Demographics.Repositories.Repositories;
//using Patient.Demographics.Commands.Categories;
using Patient.Demographics.Data.Entities.BatchProcess;
using Patient.Demographics.Commands;
using Patient.Demographics.Repositories.Dtos;
using Patient.Demographics.Service.Common;
using Patient.Demographics.Commands.Category;

namespace Patient.Demographics.Service.FileUploads.Importers.Category
{
    public class CategoryUploadImporter : IUploadImporter
    {
        private readonly IBatchProcessRepository _uploadRepository;
        private readonly IBatchProcessMasterRepository _masterRepository;
        private readonly IRowToImportMapper _rowToImportMapper;
        private readonly ICategoryImportPersistance _userImportPersistance;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IQueryModelData _queryModelData;
        private readonly ISqlDataContext _dataContext;
        private readonly IUploadsNotifier _uploadsNotifier;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IUserAccountRepository _userRepository;

        private int _totalNumberOfRecords;
        private int _numberOfRecordsUpdated;
        private Guid _uploadId;
        private int _notifierRate;

        public CategoryUploadImporter(
            IQueryModelData queryModelData,
            IBatchProcessRepository uploadRepository,
            IBatchProcessMasterRepository masterRepository,
            IRowToImportMapper rowToImportMapper,
            ICategoryImportPersistance userImportPersistance,
            ICategoryRepository categoryRepository,
            ISqlDataContext dataContext,
            IUploadsNotifier uploadsNotifier,
            ICommandExecutor commandExecutor,
            IUserAccountRepository userRepository
            )
        {
            _uploadRepository = uploadRepository;
            _masterRepository = masterRepository;
            _rowToImportMapper = rowToImportMapper;
            _userImportPersistance = userImportPersistance;
            _queryModelData = queryModelData;
            _categoryRepository = categoryRepository;
            _dataContext = dataContext;
            _uploadsNotifier = uploadsNotifier;
            _commandExecutor = commandExecutor;
            _userRepository = userRepository;
        }

        public BatchProcessTypes Handles => BatchProcessTypes.FltCategory;

        public async Task Import(Guid uploadId, Guid updatedByUserId)
        {
            var upload = await _uploadRepository.GetBatchProcessAsync(uploadId);

            if (upload == null)
            {
                throw new ApplicationException($"Import {uploadId} not found");
            }

            await _uploadsNotifier.Notify(uploadId, BatchProcessStatuses.ImportInProgress, "Saving data initiated", 1);

            var master = await _masterRepository.GetBatchProcessMasterAsync(upload.ContextId);

            var rows = await _queryModelData.BatchProcessRecordQuery.Where(r => r.BatchProcessId == uploadId && new[] { BatchProcessRecordStatuses.Header.Name, BatchProcessRecordStatuses.Ok.Name }.Contains(r.Status)).ToListAsync();

            var usersToImport = await _rowToImportMapper.Map(rows);

            await _uploadsNotifier.Notify(uploadId, BatchProcessStatuses.ImportInProgress, "Saving data initiated", 3);

            await SaveAsync(upload, master, usersToImport, updatedByUserId);

            await _uploadsNotifier.Notify(_uploadId, BatchProcessStatuses.ImportInProgress, "Processing data is completed", 100);
        }

        private async Task SaveAsync(BatchProcessDto upload, BatchProcessMasterDto orgMaster, IList<RowToImport> rowsToImport, Guid commandIssuedByUserId)
        {
            var stopwatch = Stopwatch.StartNew();

            _numberOfRecordsUpdated = 0;
            _totalNumberOfRecords = rowsToImport.Count;
            _uploadId = upload.Id;
            _notifierRate = NotificationUtilities.CalculateNotifierRate(_totalNumberOfRecords);
            var rowsProcessed = 0;

            await _uploadsNotifier.Notify(_uploadId, BatchProcessStatuses.ImportInProgress, "Saving data initiated", 5);

            var allCategories = await _categoryRepository.GetAllFltCategoriesBySupplierIdAsync(upload.SupplierId);

            var newCategories = rowsToImport.Where(c => !allCategories.Any(
                ac => c.ParsedRow[0] == ac.BiwCategory1 && c.ParsedRow[1] == ac.BiwCategory2 &&
                c.ParsedRow[2] == ac.BiwCategory3)).ToList();

            var existingCategories = rowsToImport.Where(c => allCategories.Any(
                ac => c.ParsedRow[0] == ac.BiwCategory1 && c.ParsedRow[1] == ac.BiwCategory2 &&
                c.ParsedRow[2] == ac.BiwCategory3 )).ToList();

            foreach (var record in newCategories)
            {
                var command = new Commands.Category.CreateCategoriesCommand
                {
                    BiwCategory1 = record.ParsedRow[0],
                    BiwCategory2 = record.ParsedRow[1],
                    BiwCategory3 = record.ParsedRow[2],
                    BiwCategory4 = record.ParsedRow[3],
                    MarketplacePath = record.ParsedRow[4],
                    SupplierCategory1 = record.ParsedRow[5],
                    SupplierCategory2 = record.ParsedRow[6],
                    SupplierCategory3 = record.ParsedRow[7],
                    SupplierCategory4 = record.ParsedRow[8],
                    SupplierCategory5 = record.ParsedRow[9],
                    SupplierCategory6 = record.ParsedRow[10],
                };

                command.CommandIssuedByUserId = commandIssuedByUserId;
                command.SupplierId = upload.SupplierId;
                await _commandExecutor.ExecuteAsync(command);

                await _dataContext.UpdateAsync<BatchProcessRecordEntity>(u => u.BatchProcessId == upload.Id && u.RowNumber == record.RowNumber)
                                          .Set(u => new BatchProcessRecordEntity { Status = BatchProcessRecordStatuses.Imported.Name });

                if (rowsProcessed % _notifierRate == 0)
                {
                    int percentage = (int)Math.Round((double)(100 * rowsProcessed) / _totalNumberOfRecords);

                    if (percentage < 7) percentage = 7;

                    await OutputStatusUpdate(upload.Id, rowsProcessed, _totalNumberOfRecords, "Saving data is in progress...", percentage);
                }

                rowsProcessed++;
            }

            foreach (var record in existingCategories)
            {
                var category = allCategories.FirstOrDefault(c =>
                            c.BiwCategory1 == record.ParsedRow[0] && c.BiwCategory2 == record.ParsedRow[1] &&
                            c.BiwCategory3 == record.ParsedRow[2]);

                var command = new UpdateCategoriesCommand
                {
                    MarketplacePath = record.ParsedRow[4],
                    SupplierCategory1 = record.ParsedRow[5],
                    SupplierCategory2 = record.ParsedRow[6],
                    SupplierCategory3 = record.ParsedRow[7],
                    SupplierCategory4 = record.ParsedRow[8],
                    SupplierCategory5 = record.ParsedRow[9],
                    SupplierCategory6 = record.ParsedRow[10],
                };

                command.Id = category.CategoryId;
                command.CommandIssuedByUserId = commandIssuedByUserId;
                await _commandExecutor.ExecuteAsync(command);
                await _dataContext.UpdateAsync<BatchProcessRecordEntity>(u => u.BatchProcessId == upload.Id && u.RowNumber == record.RowNumber)
                                         .Set(u => new BatchProcessRecordEntity { Status = BatchProcessRecordStatuses.Imported.Name });

                if (rowsProcessed % _notifierRate == 0)
                {
                    int percentage = (int)Math.Round((double)(100 * rowsProcessed) / _totalNumberOfRecords);

                    if (percentage < 7) percentage = 7;
                    await OutputStatusUpdate(upload.Id, rowsProcessed, _totalNumberOfRecords, "Updating data is in progress...", percentage);
                }
                rowsProcessed++;
            }
        }

        private async Task OutputStatusUpdate(Guid uploadId, int rowsProcessed, int totalRows, string message, int percentage)
        {
            DebugConsole.WriteLine($"Sending Validation Status Update {rowsProcessed}/{totalRows}");
            await _uploadsNotifier.Notify(uploadId, BatchProcessStatuses.ImportInProgress, rowsProcessed, totalRows, message, percentage);
        }
    }
}