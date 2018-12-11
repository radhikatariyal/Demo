using Patient.Demographics.Common;
using Patient.Demographics.Repository.Repositories;
using Patient.Demographics.Data;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Patient.Demographics.Repository.Dtos;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using Patient.Demographics.Repositories.Repositories;
using Patient.Demographics.Data.Entities.BatchProcess;
using Patient.Demographics.Commands;
using Patient.Demographics.Service.Common;
using Patient.Demographics.CrossCutting.Logger;
using Patient.Demographics.Data.Entities.Product;
using Patient.Demographics.Commands.Product;
using Patient.Demographics.Repositories.Dtos;
using Patient.Demographics.Commands.ProductRule;
using static Patient.Demographics.Common.Enums;
using static Patient.Demographics.Common.Rule;

namespace Patient.Demographics.Service.FileUploads.Importers.Category
{
    public class FltProductImporter : IUploadImporter
    {
        private readonly IBatchProcessRepository _batchProcessRepository;
        private readonly IBatchProcessMasterRepository _batchProcessMasterRepository;
        private readonly IRowToImportMapper _rowToImportMapper;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        //     private readonly ICatalogueRepository _catalogueRepository;
        private readonly IQueryModelData _queryModelData;
        private readonly ISqlDataContext _dataContext;
        private readonly IUploadsNotifier _uploadsNotifier;
        private readonly ICommandExecutor _commandExecutor;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IProducFileFieldsRepository _fileFieldRepository;
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;
        private int _totalNumberOfRecords;
        private int _numberOfRecordsUpdated;
        private Guid _uploadId;
        private int _notifierRate;

        private readonly ISetRepository _setRepository;
        private readonly IProductRuleRepository _productRuleRepository;

        private readonly IRuleRepository _ruleRepository;

        public FltProductImporter(
            IQueryModelData queryModelData,
            IBatchProcessRepository uploadRepository,
            IBatchProcessMasterRepository masterRepository,
            IRowToImportMapper rowToImportMapper,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            ISupplierRepository supplierRepository,
            //     ICatalogueRepository catalogueRepository,
            ISqlDataContext dataContext,
            IUploadsNotifier uploadsNotifier,
            ICommandExecutor commandExecutor, IRuleRepository ruleRepository,
        IProducFileFieldsRepository fileFieldRepository,
            ILogger logger, IUserRepository userRepository,
 ISetRepository setRepository,
        IProductRuleRepository productRuleRepository
            )
        {
            _batchProcessRepository = uploadRepository;
            _batchProcessMasterRepository = masterRepository;
            _rowToImportMapper = rowToImportMapper;
            _queryModelData = queryModelData;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            // _catalogueRepository = catalogueRepository;
            _supplierRepository = supplierRepository;
            _dataContext = dataContext;
            _uploadsNotifier = uploadsNotifier;
            _commandExecutor = commandExecutor;
            _fileFieldRepository = fileFieldRepository;
            _logger = logger;
            _userRepository = userRepository;
            _ruleRepository = ruleRepository;
            _setRepository = setRepository;
            _productRuleRepository = productRuleRepository;
        }

        public BatchProcessTypes Handles => BatchProcessTypes.FltProduct;

        public async Task Import(Guid uploadId, Guid updatedByUserId)
        {
            var upload = await _batchProcessRepository.GetBatchProcessAsync(uploadId);

            if (upload == null)
            {
                throw new ApplicationException($"Import {uploadId} not found");
            }

            await _uploadsNotifier.Notify(uploadId, BatchProcessStatuses.ImportInProgress, "Saving data initiated", 1);

            var master = await _batchProcessMasterRepository.GetBatchProcessMasterAsync(upload.ContextId);

            var rows = await _queryModelData.BatchProcessRecordQuery.Where(r => r.BatchProcessId == uploadId && new[] { BatchProcessRecordStatuses.Header.Name, BatchProcessRecordStatuses.Ok.Name }.Contains(r.Status)).ToListAsync();

            var usersToImport = await _rowToImportMapper.Map(rows);

            await _uploadsNotifier.Notify(uploadId, BatchProcessStatuses.ImportInProgress, "Saving data initiated", 3);

            await SaveAsync(upload, master, usersToImport, updatedByUserId);

            await _uploadsNotifier.Notify(_uploadId, BatchProcessStatuses.ImportInProgress, "Processing data is completed", 100);
        }

        private async Task SaveAsync(BatchProcessDto upload, BatchProcessMasterDto orgMaster, IList<RowToImport> rowsToImport, Guid commandIssuedByUserId)
        {
            var stopwatch = Stopwatch.StartNew();
            //List<Productdto> Products = new List<Productdto>();
            _numberOfRecordsUpdated = 0;
            _totalNumberOfRecords = rowsToImport.Count;
            _uploadId = upload.Id;
            _notifierRate = NotificationUtilities.CalculateNotifierRate(_totalNumberOfRecords);
            var rowsProcessed = 0;
            var data = await _fileFieldRepository.GetFieldsByIdOrder(upload.ProductFileId);
            var fields = data.productFileFieldsdata;
            int BICostIndex = fields.FindIndex(x => x.fieldName == "BI Cost");
            int BrandIndex = fields.FindIndex(x => x.fieldName == "Brand");
            int Category1Index = fields.FindIndex(x => x.fieldName == "Category 1");
            int Category2Index = fields.FindIndex(x => x.fieldName == "Category 1");
            int Category3Index = fields.FindIndex(x => x.fieldName == "Category 1");
            int LeadDaysIndex = fields.FindIndex(x => x.fieldName == "Lead Days");
            int ModelNumberIndex = fields.FindIndex(x => x.fieldName == "Model Number");
            int MSRPIndex = fields.FindIndex(x => x.fieldName == "MSRP");
            int StatusIndex = fields.FindIndex(x => x.fieldName == "Status");
            int StatusDateIndex = fields.FindIndex(x => x.fieldName == "Status Date");
            int VendorIDIndex = fields.FindIndex(x => x.fieldName == "Vendor ID");
            int VendorProductIDIndex = fields.FindIndex(x => x.fieldName == "Vendor Product ID");
            int NameIndex = fields.FindIndex(x => x.fieldName == "Name");
            int DescriptionIndex = fields.FindIndex(x => x.fieldName == "Description");
            int UPCCodeIndex = fields.FindIndex(x => x.fieldName == "UPC Code");
            await _uploadsNotifier.Notify(_uploadId, BatchProcessStatuses.ImportInProgress, "Saving data initiated", 5);
            var user = await _userRepository.GetUserByIdAsync(commandIssuedByUserId);
            
            if (upload.SupplierId!=null)
            {
                var supplier = await _supplierRepository.GetSupplierByIdAsync(upload.SupplierId);
                if (supplier.ProductFileSpecificationId.HasValue)
                {

                    var rules = await _fileFieldRepository.GetProductRulesById(upload.ProductFileId);

                    foreach (var record in rowsToImport)
                    {
                        try
                        {
                            List<Productdto> product = new List<Productdto>();
                            foreach (var item in record.ParsedRow)
                            {
                                product.Add(new Productdto { value = item });
                            }
                            var error= _productRepository.validateProduct("FltProduct", commandIssuedByUserId, supplier.Id, product, data.productFileFieldsdata, rules);
                            if (error.Count == 0)
                            {
                                var index = _productRepository.getFieldsIndex(data.productFileFieldsdata);
                                bool isExist = false;
                                if ((int)ProductUniqueId.VendorProductId == supplier.ProductUniqueId)
                                {
                                    isExist = await _productRepository.IsFilealreadyExists(supplier.Id, supplier.ProductUniqueId, product[index["VendorProductIDIndex"]].value);
                                }
                                if ((int)ProductUniqueId.ModelNumber == supplier.ProductUniqueId)
                                {
                                    isExist = await _productRepository.IsFilealreadyExists(supplier.Id, supplier.ProductUniqueId, product[index["ModelNumberIndex"]].value);

                                }
                                if ((int)ProductUniqueId.UPCCode == supplier.ProductUniqueId)
                                {
                                    isExist = await _productRepository.IsFilealreadyExists(supplier.Id, supplier.ProductUniqueId, product[index["UPCCodeIndex"]].value);

                                }
                                if (!isExist)
                                {
                                    await Createproduct("FltProduct", commandIssuedByUserId, supplier.Id, product, data.productFileFieldsdata, rules);
                                    if (!await ApplyRule(supplier, product, data.productFileFieldsdata, commandIssuedByUserId, "Create"))
                                    {

                                        return;
                                    }
                                }
                                else
                                {
                                    // index = _productRepository.getFieldsIndex(data.productFileFieldsdata);
                                    var allProducts = await _productRepository.GetAllProductsAsync(supplier.Id);
                                    if ((int)ProductUniqueId.VendorProductId == supplier.ProductUniqueId)
                                    {
                                        var newProducts = product.Where(c =>
                                      !allProducts.Any(ac => product[index["VendorProductIDIndex"]].value == ac.VendorProductId)).ToList();

                                        var existingProducts = product.Where(c =>
                                            allProducts.Any(ac => product[index["VendorProductIDIndex"]].value == ac.VendorProductId)).ToList();

                                        var deleterow = allProducts.Where(p => existingProducts.Any(l => p.VendorProductId == l.value)).ToList();

                                        //if (deleterow.Count > 0)
                                        //{
                                        //    await this.deleteRecords(deleterow, commandIssuedByUserId);
                                        //}
                                        if (newProducts.Count > 0)
                                        {
                                            await Createproduct("FltProduct", commandIssuedByUserId, supplier.Id, product, data.productFileFieldsdata, rules);
                                            if(!await ApplyRule(supplier, product, data.productFileFieldsdata, commandIssuedByUserId, "Create")) {

                                                return ;
                                            }

                                        }
                                        if (existingProducts.Count > 0)
                                        {
                                            await this.Updateproduct("FltProduct", commandIssuedByUserId, supplier.Id, existingProducts, data.productFileFieldsdata, rules, ProductUniqueId.VendorProductId, ProcessStatus.Pending);
                                            if(!await ApplyRule(supplier, product, data.productFileFieldsdata, commandIssuedByUserId, "Update")) return;

                                        }
                                    }
                                    if ((int)ProductUniqueId.ModelNumber == supplier.ProductUniqueId)
                                    {
                                        var newProducts = product.Where(c =>
                                      !allProducts.Any(ac => product[index["ModelNumberIndex"]].value == ac.ModelNumber)).ToList();

                                        var existingProducts = product.Where(c =>
                                            allProducts.Any(ac => product[index["ModelNumberIndex"]].value == ac.ModelNumber)).ToList();

                                        var deleterow = allProducts.Where(p => existingProducts.Any(l => p.VendorProductId == l.value)).ToList();

                                        //if (deleterow.Count > 0)
                                        //{
                                        //    await this.deleteRecords(deleterow, commandIssuedByUserId);
                                        //}
                                        if (newProducts.Count > 0)
                                        {
                                            await Createproduct("FltProduct", commandIssuedByUserId, supplier.Id, product, data.productFileFieldsdata, rules);
                                            if(!await ApplyRule(supplier, product, data.productFileFieldsdata, commandIssuedByUserId, "Create")) return;

                                        }
                                        if (existingProducts.Count > 0)
                                        {
                                            await this.Updateproduct("FltProduct", commandIssuedByUserId, supplier.Id, existingProducts, data.productFileFieldsdata, rules, ProductUniqueId.ModelNumber,ProcessStatus.Pending);
                                            if(!await ApplyRule(supplier, product, data.productFileFieldsdata, commandIssuedByUserId, "Update")) return;

                                        }
                                    }
                                    if ((int)ProductUniqueId.UPCCode == supplier.ProductUniqueId)
                                    {
                                        var newProducts = product.Where(c =>
                                      !allProducts.Any(ac => product[index["UPCCodeIndex"]].value == ac.UPCCode)).ToList();

                                        var existingProducts = product.Where(c =>
                                            allProducts.Any(ac => product[index["UPCCodeIndex"]].value == ac.UPCCode)).ToList();

                                        var deleterow = allProducts.Where(p => existingProducts.Any(l => p.VendorProductId == l.value)).ToList();

                                        //if (deleterow.Count > 0)
                                        //{
                                        //    await this.deleteRecords(deleterow, commandIssuedByUserId);
                                        //}
                                        if (newProducts.Count > 0)
                                        {
                                            await Createproduct("FltProduct", commandIssuedByUserId, supplier.Id, product, data.productFileFieldsdata, rules);
                                            if (!await ApplyRule(supplier, product, data.productFileFieldsdata, commandIssuedByUserId, "Create")) return;

                                        }
                                        if (existingProducts.Count > 0)
                                        {
                                            await this.Updateproduct("FltProduct", commandIssuedByUserId, supplier.Id, existingProducts, data.productFileFieldsdata, rules, ProductUniqueId.UPCCode, ProcessStatus.Pending);
                                            if (!await ApplyRule(supplier, product, data.productFileFieldsdata, commandIssuedByUserId, "Update")) return;

                                        }
                                    }
                                }
                              
                                if (rowsProcessed % _notifierRate == 0)
                                {
                                    int percentage = (int)Math.Round((double)(100 * rowsProcessed) / _totalNumberOfRecords);

                                    if (percentage < 7) percentage = 7;

                                    await OutputStatusUpdate(upload.Id, rowsProcessed, _totalNumberOfRecords, "Saving data is in progress...", percentage);
                                }

                                rowsProcessed++;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogException(ex);
                            // _logger.LogInfo("Error while insert FLT product :: " + record.ParsedRow[3].Trim() + " ::" + ex.Message);
                        }
                    }
                }
            }
        }
        //else
        //{
        //    var allProducts = await _productRepository.GetAllProductsAsync(supplier.Id);
        //    if ((int)ProductUniqueId.ModelNumber == supplier.ProductUniqueId)
        //    {
        //        var newProducts = rowsToImport.Where(c =>
        //      !allProducts.Any(ac => c.ParsedRow[ModelNumberIndex] == ac.ModelNumber)).ToList();

        //        var existingProducts = rowsToImport.Where(c =>
        //            allProducts.Any(ac => c.ParsedRow[ModelNumberIndex] == ac.ModelNumber)).ToList();

        //        if (newProducts.Count > 0)
        //            await this.createCommand(newProducts, data, supplier, commandIssuedByUserId);
        //        if (existingProducts.Count > 0)
        //            await this.updateRecords(existingProducts, data, supplier, commandIssuedByUserId, ProductUniqueId.ModelNumber);
        //    }


        //    else if ((int)ProductUniqueId.VendorProductId == supplier.ProductUniqueId)
        //    {
        //        var newProducts = rowsToImport.Where(c =>
        //      !allProducts.Any(ac => c.ParsedRow[VendorProductIDIndex] == ac.VendorProductId)).ToList();

        //        var existingProducts = rowsToImport.Where(c =>
        //            allProducts.Any(ac => c.ParsedRow[VendorProductIDIndex] == ac.VendorProductId)).ToList();



        //        var deleterow = allProducts.Where(p => !existingProducts.Any(l => p.VendorProductId == l.ParsedRow[VendorProductIDIndex])).ToList(); ;

        //        if (deleterow.Count > 0)
        //        {

        //            await this.deleteRecords(deleterow, commandIssuedByUserId);
        //        }
        //        if (newProducts.Count > 0)
        //            await this.createCommand(newProducts, data, supplier, commandIssuedByUserId);
        //        if (existingProducts.Count > 0)
        //            await this.updateRecords(existingProducts, data, supplier, commandIssuedByUserId, ProductUniqueId.VendorProductId
        //                );
        //    }
        //    else
        //    {
        //    }

        //}
        public async Task Updateproduct(string uploadType, Guid id, Guid supplierId, List<Productdto> Products, List<ProductFileFieldsDto> Fields, IList<BatchProductRulesDto> rules, ProductUniqueId uniqueId, ProcessStatus status)
        {
            var command = new Commands.Product.UpdateFltProductCommand();
            var result = await _productRepository.ProcessUpdateProduct("FltProduct", id, supplierId, Products, Fields, rules, uniqueId);
            if (result != null)
            {
                ProductProcessRecordDto dto = new ProductProcessRecordDto();
                dto = result;
                command = this.Mapupdate(dto);
                command.CommandIssuedByUserId = id;
                command.ProcessStatus = status;
                command.UpdateStatus = UpdateStatus.New;
                await _commandExecutor.ExecuteAsync(command);
            }
        }
        public async Task Createproduct(string uploadType, Guid id, Guid supplierId, List<Productdto> Products, List<ProductFileFieldsDto> Fields, IList<BatchProductRulesDto> rules)
        {
            var command = new Commands.Product.CreateFltProductCommand();
            var result = _productRepository.ProcessProduct("FltProduct", id, supplierId, Products, Fields, rules);
            if (result != null)
            {
                ProductProcessRecordDto dto = new ProductProcessRecordDto();
                dto = result;
                command = this.Map(dto);
                command.CommandIssuedByUserId = id;
                command.ProcessStatus = ProcessStatus.Pending;
                command.UpdateStatus = UpdateStatus.New;
                await _commandExecutor.ExecuteAsync(command);
            }
        }
        public CreateFltProductCommand Map(ProductProcessRecordDto dto)
        {
            return new CreateFltProductCommand
            {
                Id = dto.Id,
                SupplierId = dto.SupplierId,
                StatusDate = dto.StatusDate,
                Status = dto.Status,
                VendorId = dto.VendorId,
                VendorProductId = dto.VendorProductId,
                ModelNumber = dto.ModelNumber,
                Category1 = dto.Category1,
                Category2 = dto.Category2,
                Category3 = dto.Category3,
                Brand = dto.Brand,
                MSRP = dto.MSRP,
                BIcost = dto.BIcost,
                LeadDays = dto.LeadDays,
                UPCCode = dto.UPCCode,
                Name=dto.Name,
                Description=dto.Description,
                ProductCountry = dto.ProductCountry,
                ProductLanguage = dto.ProductLanguage,
                ProductOptionalField = dto.ProductOptionalField,
                ProductAdditionalField = dto.ProductAdditionalField
            };
        }

        public UpdateFltProductCommand Mapupdate(ProductProcessRecordDto dto)
        {
            return new UpdateFltProductCommand
            {
                Id = dto.Id,
                SupplierId = dto.SupplierId,
                StatusDate = dto.StatusDate,
                Status = dto.Status,
                VendorId = dto.VendorId,
                VendorProductId = dto.VendorProductId,
                ModelNumber = dto.ModelNumber,
                Category1 = dto.Category1,
                Category2 = dto.Category2,
                Category3 = dto.Category3,
                Brand = dto.Brand,
                MSRP = dto.MSRP,
                BIcost = dto.BIcost,
                LeadDays = dto.LeadDays,
                UPCCode = dto.UPCCode,
                Name = dto.Name,
                Description = dto.Description,
                ProductCountry = dto.ProductCountry,
                ProductLanguage = dto.ProductLanguage,
                ProductOptionalField = dto.ProductOptionalField,
                ProductAdditionalField = dto.ProductAdditionalField
            };
        }
        public async Task<IList<RuleDto>> getRules(Guid SupplierId)
        {
            var supplier = await this._supplierRepository.GetSupplierByIdAsync(SupplierId);
            var set = this._setRepository.GetSetByIdAsync(supplier.SetId);
            return await this._ruleRepository.GetRules(supplier.SetId, SupplierId);
        }
        public async Task<Boolean> ApplyRule(SupplierDto supplier, List<Productdto> products, List<ProductFileFieldsDto> fields, Guid id, string action)
        {
            var ruleList = await getRules(supplier.Id);
            var rules = await _fileFieldRepository.GetProductRulesById(supplier.ProductFileSpecificationId.Value);

            ProductProcessRecordDto dto = await _productRepository.getProductRecords(supplier.ProductUniqueId, products, fields);
            if (dto != null)
            {
                if (ruleList.Count > 0)
                {
                    List<ProductRuleDto> prodtRule = await _productRuleRepository.ApplyRules(ruleList, dto);
                    if (prodtRule.Count != 0)
                    {
                        if (action == "Create")
                            await CreateProductRule(prodtRule, id);
                        else
                        {
                            await UpdateProductRule(prodtRule, id);
                            await CreateProductRule(prodtRule, id);
                        }
                    }
                }
                RuleColor rule = await GetProcessStatus(dto.Id);
                if ((int)ProductUniqueId.VendorProductId == supplier.ProductUniqueId)
                {
                    if (rule == RuleColor.Green)
                        await this.Updateproduct("FltProduct", id, supplier.Id, products, fields, rules, ProductUniqueId.VendorProductId, ProcessStatus.Green);
                    else if (rule == RuleColor.Yellow)
                        await this.Updateproduct("FltProduct", id, supplier.Id, products, fields, rules, ProductUniqueId.VendorProductId, ProcessStatus.Yellow);
                    else
                        await this.Updateproduct("FltProduct", id, supplier.Id, products, fields, rules, ProductUniqueId.VendorProductId, ProcessStatus.Red);
                }
                if ((int)ProductUniqueId.ModelNumber == supplier.ProductUniqueId)
                {
                    if (rule == RuleColor.Green)
                        await this.Updateproduct("FltProduct", id, supplier.Id, products, fields, rules, ProductUniqueId.ModelNumber, ProcessStatus.Green);
                    else if (rule == RuleColor.Yellow)
                        await this.Updateproduct("FltProduct", id, supplier.Id, products, fields, rules, ProductUniqueId.ModelNumber, ProcessStatus.Yellow);
                    else
                        await this.Updateproduct("FltProduct", id, supplier.Id, products, fields, rules, ProductUniqueId.ModelNumber, ProcessStatus.Red);
                }
                if ((int)ProductUniqueId.UPCCode == supplier.ProductUniqueId)
                {
                    if (rule == RuleColor.Green)
                        await this.Updateproduct("FltProduct", id, supplier.Id, products, fields, rules, ProductUniqueId.UPCCode, ProcessStatus.Green);
                    else if (rule == RuleColor.Yellow)
                        await this.Updateproduct("FltProduct", id, supplier.Id, products, fields, rules, ProductUniqueId.UPCCode, ProcessStatus.Yellow);
                    else
                        await this.Updateproduct("FltProduct", id, supplier.Id, products, fields, rules, ProductUniqueId.UPCCode, ProcessStatus.Red);
                }
                return true;
            }
            return false;
        }
        public async Task<RuleColor> GetProcessStatus(Guid productId)
        {
            var item = await _productRuleRepository.getRuleAppliedforProductAsync(productId);
            Dictionary<RuleColor, int> RuleColorList = new Dictionary<RuleColor, int>();
            int count = 0;
            if (item.Count > 0)
            {
                foreach (var i in item)
                {
                    count++;
                    var rule = await _ruleRepository.GetRules(i.RuleId);
                    if (RuleColorList.ContainsKey(rule.Color))
                    {
                        RuleColorList[rule.Color] = count;
                    }
                    else
                        RuleColorList.Add(rule.Color, count);

                }
                if (RuleColorList.Count == 1)
                {
                    foreach (var rulColor in RuleColorList)
                    {
                        if (rulColor.Key == RuleColor.Red)
                        {
                            return RuleColor.Red;
                        }
                        else
                            return RuleColor.Yellow;
                    }
                }
                else
                    return RuleColor.Red;
            }
            else
                return RuleColor.Green;
            return RuleColor.Green;
        }
        public async Task CreateProductRule(List<ProductRuleDto> prodtRule, Guid id)
        {
            foreach (var prodRule in prodtRule)
            {
                var command = new CreateProductRuleCommand
                {

                    ProductId = prodRule.ProducdId,
                    RuleId = prodRule.RuleId
                };
                command.CommandIssuedByUserId = id;
                await _commandExecutor.ExecuteAsync(command);
            }
        }
        public async Task UpdateProductRule(List<ProductRuleDto> prodtRule, Guid id)
        {
            var item = await _productRuleRepository.getRuleAppliedforProductAsync(prodtRule[0].ProducdId);

            foreach (var prod in item)
            {
                var command = new DeleteProductRuleCommand
                {

                    Id = prod.Id,
                };
                command.CommandIssuedByUserId = id;
                await _commandExecutor.ExecuteAsync(command);
            }
        }

        //public async Task createCommand(IList<RowToImport> rowsToImport, ProductFileFieldData data, Repositories.Dtos.SupplierDto supplier, Guid commandIssuedByUserId)
        //{
        //    var fields = data.productFileFieldsdata;
        //    int BICostIndex = fields.FindIndex(x => x.fieldName == "BI Cost");
        //    int BrandIndex = fields.FindIndex(x => x.fieldName == "Brand");
        //    int Category1Index = fields.FindIndex(x => x.fieldName == "Category 1");
        //    int Category2Index = fields.FindIndex(x => x.fieldName == "Category 1");
        //    int Category3Index = fields.FindIndex(x => x.fieldName == "Category 1");
        //    int LeadDaysIndex = fields.FindIndex(x => x.fieldName == "Lead Days");
        //    int ModelNumberIndex = fields.FindIndex(x => x.fieldName == "Model Number");
        //    int MSRPIndex = fields.FindIndex(x => x.fieldName == "MSRP");
        //    int StatusIndex = fields.FindIndex(x => x.fieldName == "Status");
        //    int StatusDateIndex = fields.FindIndex(x => x.fieldName == "Status Date");
        //    int VendorIDIndex = fields.FindIndex(x => x.fieldName == "Vendor ID");
        //    int VendorProductIDIndex = fields.FindIndex(x => x.fieldName == "Vendor Product ID");
        //    foreach (var record in rowsToImport)
        //    {
        //        try
        //        {
        //            var countryFields = fields.FindAll(x => x.fieldType == "Country");
        //            var languageFields = fields.FindAll(x => x.fieldType == "Language");
        //            var optionalFields = fields.FindAll(x => x.fieldType == "" && x.isOptional == true);
        //            var command = new Commands.Product.CreateFltProductCommand
        //            {
        //                Id = Identifier.NewId(),
        //                BIcost = record.ParsedRow[BICostIndex].Trim(),
        //                Brand = record.ParsedRow[BrandIndex].Trim(),
        //                Category1 = record.ParsedRow[Category1Index].Trim(),
        //                Category2 = record.ParsedRow[Category2Index].Trim(),
        //                Category3 = record.ParsedRow[Category3Index].Trim(),
        //                LeadDays = record.ParsedRow[LeadDaysIndex].Trim(),
        //                ModelNumber = record.ParsedRow[ModelNumberIndex].Trim(),
        //                MSRP = record.ParsedRow[MSRPIndex].Trim(),
        //                Status = record.ParsedRow[StatusIndex].Trim(),
        //                StatusDate = record.ParsedRow[StatusDateIndex].Trim(),
        //                VendorId = record.ParsedRow[VendorIDIndex].Trim(),
        //                VendorProductId = record.ParsedRow[VendorProductIDIndex].Trim(),

        //            };
        //            List<ProductCountryDto> productCountry = new List<ProductCountryDto>();
        //            if (countryFields.Count > 0)
        //            {
        //                ProductCountryDto Country = null; int CountryAdditionCostIndex = -1; int CountryFrieghtAmountIndex = -1; int CountryDropShipFeeIndex = -1; int CountryTaxAmountIndex = -1;
        //                for (int i = 0; i < countryFields.Count; i = i + 4)
        //                {
        //                    string isoCode = countryFields[i].fieldName.Remove(countryFields[i].fieldName.LastIndexOf("-Additional Cost"));

        //                    if (countryFields[i].fieldName.EndsWith("-Additional Cost"))
        //                        CountryAdditionCostIndex = countryFields[i].sortOrderNum;
        //                    if (countryFields[i + 1].fieldName.EndsWith("-Freight Amount"))
        //                        CountryFrieghtAmountIndex = countryFields[i + 1].sortOrderNum;
        //                    if (countryFields[i + 2].fieldName.EndsWith("-Drop Ship Fee"))
        //                        CountryDropShipFeeIndex = countryFields[i + 2].sortOrderNum;
        //                    if (countryFields[i + 3].fieldName.EndsWith("-Tax Amount"))
        //                        CountryTaxAmountIndex = countryFields[i + 3].sortOrderNum;

        //                    Country = new ProductCountryDto();
        //                    Country.Id = Identifier.NewId();
        //                    Country.IsoCode = isoCode;
        //                    Country.ProductId = command.Id;
        //                    Country.AdditionalCost = record.ParsedRow[CountryAdditionCostIndex].Trim();
        //                    Country.DropShipFee = record.ParsedRow[CountryDropShipFeeIndex].Trim();
        //                    Country.FrieghtAmount = record.ParsedRow[CountryFrieghtAmountIndex].Trim();
        //                    Country.TaxAmount = record.ParsedRow[CountryTaxAmountIndex].Trim();
        //                    productCountry.Add(Country);
        //                }
        //            }
        //            List<ProductLanguageDto> productLanguage = new List<ProductLanguageDto>();
        //            if (languageFields.Count > 0)
        //            {
        //                ProductLanguageDto Language = null; int LanguageNameIndex = -1; int LanguageDescriptionIndex = -1;
        //                for (int i = 0; i < languageFields.Count; i = i + 2)
        //                {
        //                    string SpecName = languageFields[i].fieldName.Remove(languageFields[i].fieldName.LastIndexOf("-Name"));

        //                    if (languageFields[i].fieldName.EndsWith("-Name"))
        //                        LanguageNameIndex = languageFields[i].sortOrderNum;
        //                    if (languageFields[i + 1].fieldName.EndsWith("-Description"))
        //                        LanguageDescriptionIndex = languageFields[i + 1].sortOrderNum;


        //                    Language = new ProductLanguageDto();
        //                    Language.Id = Identifier.NewId();
        //                    Language.Code = SpecName;
        //                    Language.ProductId = command.Id;
        //                    Language.Name = record.ParsedRow[LanguageNameIndex].Trim();
        //                    Language.Description = record.ParsedRow[LanguageDescriptionIndex].Trim();

        //                    productLanguage.Add(Language);
        //                }
        //            }
        //            List<ProductOptionalFieldsDto> productOptionalField = new List<ProductOptionalFieldsDto>();
        //            if (optionalFields.Count > 0)
        //            {
        //                ProductOptionalFieldsDto optionalfield = null; int KeyIndex = -1; int ValueIndex = -1;
        //                for (int i = 0; i < optionalFields.Count; i++)
        //                {

        //                    //if (optionalFields[i].fieldName.EndsWith("-Name"))
        //                    KeyIndex = optionalFields[i].sortOrderNum;
        //                    string Key = optionalFields[i].fieldName;
        //                    //if (languageFields[i + 1].fieldName.EndsWith("-Description"))
        //                    //   LanguageDescriptionIndex = languageFields[i + 1].sortOrderNum;


        //                    optionalfield = new ProductOptionalFieldsDto();
        //                    optionalfield.Id = Identifier.NewId();
        //                    optionalfield.ProductId = command.Id;
        //                    optionalfield.Key = Key;
        //                    optionalfield.Value = record.ParsedRow[KeyIndex].Trim();

        //                    productOptionalField.Add(optionalfield);
        //                }
        //            }
        //            command.ProductCountry = productCountry;
        //            command.ProductLanguage = productLanguage;
        //            command.ProductOptionalField = productOptionalField;
        //            command.SupplierId = supplier.Id;
        //            command.CommandIssuedByUserId = commandIssuedByUserId;
        //            await _commandExecutor.ExecuteAsync(command);



        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogException(ex);
        //            // _logger.LogInfo("Error while insert FLT product :: " + record.ParsedRow[3].Trim() + " ::" + ex.Message);
        //        }
        //    }
        //}
        //public async Task updateRecords(IList<RowToImport> newProducts, ProductFileFieldData data, Repositories.Dtos.SupplierDto supplier, Guid commandIssuedByUserId, ProductUniqueId uniqueId)
        //{
        //    var fields = data.productFileFieldsdata;
        //    int BICostIndex = fields.FindIndex(x => x.fieldName == "BI Cost");
        //    int BrandIndex = fields.FindIndex(x => x.fieldName == "Brand");
        //    int Category1Index = fields.FindIndex(x => x.fieldName == "Category 1");
        //    int Category2Index = fields.FindIndex(x => x.fieldName == "Category 1");
        //    int Category3Index = fields.FindIndex(x => x.fieldName == "Category 1");
        //    int LeadDaysIndex = fields.FindIndex(x => x.fieldName == "Lead Days");
        //    int ModelNumberIndex = fields.FindIndex(x => x.fieldName == "Model Number");
        //    int MSRPIndex = fields.FindIndex(x => x.fieldName == "MSRP");
        //    int StatusIndex = fields.FindIndex(x => x.fieldName == "Status");
        //    int StatusDateIndex = fields.FindIndex(x => x.fieldName == "Status Date");
        //    int VendorIDIndex = fields.FindIndex(x => x.fieldName == "Vendor ID");
        //    int VendorProductIDIndex = fields.FindIndex(x => x.fieldName == "Vendor Product ID");
        //    Guid id; ProductProcessRecordDto dto = new ProductProcessRecordDto();
        //    foreach (var record in newProducts)
        //    {
        //        if (uniqueId == ProductUniqueId.ModelNumber)
        //        {
        //            var modelnum = record.ParsedRow[ModelNumberIndex].Trim();
        //            dto = await _productRepository.GetProductsByUniqueIdAsync(ProductUniqueId.ModelNumber, modelnum);
        //        }

        //        if (uniqueId == ProductUniqueId.VendorProductId)
        //        {
        //            var VendeorProdId = record.ParsedRow[VendorProductIDIndex].Trim();
        //            dto = await _productRepository.GetProductsByUniqueIdAsync(ProductUniqueId.VendorProductId, VendeorProdId);
        //        }

        //        try
        //        {
        //            var countryFields = fields.FindAll(x => x.fieldType == "Country");
        //            var languageFields = fields.FindAll(x => x.fieldType == "Language");
        //            var optionalFields = fields.FindAll(x => x.fieldType == "" && x.isOptional == true);

        //            var command = new Commands.Product.UpdateFltProductCommand
        //            {
        //                Id = dto.Id,
        //                BIcost = record.ParsedRow[BICostIndex].Trim(),
        //                Brand = record.ParsedRow[BrandIndex].Trim(),
        //                Category1 = record.ParsedRow[Category1Index].Trim(),
        //                Category2 = record.ParsedRow[Category2Index].Trim(),
        //                Category3 = record.ParsedRow[Category3Index].Trim(),
        //                LeadDays = record.ParsedRow[LeadDaysIndex].Trim(),
        //                ModelNumber = record.ParsedRow[ModelNumberIndex].Trim(),
        //                MSRP = record.ParsedRow[MSRPIndex].Trim(),
        //                Status = record.ParsedRow[StatusIndex].Trim(),
        //                StatusDate = record.ParsedRow[StatusDateIndex].Trim(),
        //                VendorId = record.ParsedRow[VendorIDIndex].Trim(),
        //                VendorProductId = record.ParsedRow[VendorProductIDIndex].Trim(),

        //            };
        //            //  var country = await _Pr
        //            List<ProductCountryDto> productCountry = new List<ProductCountryDto>();
        //            if (countryFields.Count > 0)
        //            {
        //                ProductCountryDto Country = null; int CountryAdditionCostIndex = -1; int CountryFrieghtAmountIndex = -1; int CountryDropShipFeeIndex = -1; int CountryTaxAmountIndex = -1;
        //                for (int i = 0; i < countryFields.Count; i = i + 4)
        //                {
        //                    string isoCode = countryFields[i].fieldName.Remove(countryFields[i].fieldName.LastIndexOf("-Additional Cost"));

        //                    if (countryFields[i].fieldName.EndsWith("-Additional Cost"))
        //                        CountryAdditionCostIndex = countryFields[i].sortOrderNum;
        //                    if (countryFields[i + 1].fieldName.EndsWith("-Freight Amount"))
        //                        CountryFrieghtAmountIndex = countryFields[i + 1].sortOrderNum;
        //                    if (countryFields[i + 2].fieldName.EndsWith("-Drop Ship Fee"))
        //                        CountryDropShipFeeIndex = countryFields[i + 2].sortOrderNum;
        //                    if (countryFields[i + 3].fieldName.EndsWith("-Tax Amount"))
        //                        CountryTaxAmountIndex = countryFields[i + 3].sortOrderNum;

        //                    Country = new ProductCountryDto();
        //                    Country.Id = Identifier.NewId();
        //                    Country.IsoCode = isoCode;
        //                    Country.ProductId = command.Id;
        //                    Country.AdditionalCost = record.ParsedRow[CountryAdditionCostIndex].Trim();
        //                    Country.DropShipFee = record.ParsedRow[CountryDropShipFeeIndex].Trim();
        //                    Country.FrieghtAmount = record.ParsedRow[CountryFrieghtAmountIndex].Trim();
        //                    Country.TaxAmount = record.ParsedRow[CountryTaxAmountIndex].Trim();
        //                    productCountry.Add(Country);
        //                }
        //            }
        //            List<ProductLanguageDto> productLanguage = new List<ProductLanguageDto>();
        //            if (languageFields.Count > 0)
        //            {
        //                ProductLanguageDto Language = null; int LanguageNameIndex = -1; int LanguageDescriptionIndex = -1;
        //                for (int i = 0; i < languageFields.Count; i = i + 2)
        //                {
        //                    string SpecName = languageFields[i].fieldName.Remove(languageFields[i].fieldName.LastIndexOf("-Name"));

        //                    if (languageFields[i].fieldName.EndsWith("-Name"))
        //                        LanguageNameIndex = languageFields[i].sortOrderNum;
        //                    if (languageFields[i + 1].fieldName.EndsWith("-Description"))
        //                        LanguageDescriptionIndex = languageFields[i + 1].sortOrderNum;


        //                    Language = new ProductLanguageDto();
        //                    Language.Id = Identifier.NewId();
        //                    Language.Code = SpecName;
        //                    Language.ProductId = command.Id;
        //                    Language.Name = record.ParsedRow[LanguageNameIndex].Trim();
        //                    Language.Description = record.ParsedRow[LanguageDescriptionIndex].Trim();

        //                    productLanguage.Add(Language);
        //                }
        //            }
        //            List<ProductOptionalFieldsDto> productOptionalField = new List<ProductOptionalFieldsDto>();
        //            if (optionalFields.Count > 0)
        //            {
        //                ProductOptionalFieldsDto optionalfield = null; int KeyIndex = -1; int ValueIndex = -1;
        //                for (int i = 0; i < optionalFields.Count; i++)
        //                {

        //                    //if (optionalFields[i].fieldName.EndsWith("-Name"))
        //                    KeyIndex = optionalFields[i].sortOrderNum;
        //                    string Key = optionalFields[i].fieldName;
        //                    //if (languageFields[i + 1].fieldName.EndsWith("-Description"))
        //                    //   LanguageDescriptionIndex = languageFields[i + 1].sortOrderNum;


        //                    optionalfield = new ProductOptionalFieldsDto();
        //                    optionalfield.Id = Identifier.NewId();
        //                    optionalfield.ProductId = command.Id;
        //                    optionalfield.Key = Key;
        //                    optionalfield.Value = record.ParsedRow[KeyIndex].Trim();

        //                    productOptionalField.Add(optionalfield);
        //                }
        //            }
        //            command.ProductCountry = productCountry;
        //            command.ProductLanguage = productLanguage;
        //            command.ProductOptionalField = productOptionalField;
        //            command.SupplierId = supplier.Id;
        //            command.CommandIssuedByUserId = commandIssuedByUserId;
        //            await _commandExecutor.ExecuteAsync(command);



        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogException(ex);
        //            // _logger.LogInfo("Error while insert FLT product :: " + record.ParsedRow[3].Trim() + " ::" + ex.Message);
        //        }
        //    }
        //}

        public async Task deleteRecords(IList<ProductProcessRecordDto> Products, Guid commandIssuedByUserId)
        {
            foreach (var record in Products)
            {


                try
                {

                    var command = new Commands.Product.DeleteFltProductCommand
                    {
                        Id = record.Id,
                        BIcost = record.BIcost,
                        Brand = record.Brand,
                        Category1 = record.Category1,
                        Category2 = record.Category2,
                        Category3 = record.Category3,
                        LeadDays = record.LeadDays,
                        ModelNumber = record.ModelNumber,
                        MSRP = record.MSRP,
                        Status = record.Status,
                        StatusDate = record.StatusDate,
                        VendorId = record.VendorId,
                        VendorProductId = record.VendorProductId,
                        Name = record.Name,
                        Description=record.Description,
                        ProductCountry = record.ProductCountry,
                        ProductLanguage = record.ProductLanguage,
                        ProductOptionalField = record.ProductOptionalField
                    };

                    command.CommandIssuedByUserId = commandIssuedByUserId;
                    await _commandExecutor.ExecuteAsync(command);

                }
                catch (Exception ex)
                {
                    _logger.LogException(ex);
                    // _logger.LogInfo("Error while insert FLT product :: " + record.ParsedRow[3].Trim() + " ::" + ex.Message);

                }
            }
        }
        private async Task OutputStatusUpdate(Guid uploadId, int rowsProcessed, int totalRows, string message, int percentage)
        {
            DebugConsole.WriteLine($"Sending Validation Status Update {rowsProcessed}/{totalRows}");
            await _uploadsNotifier.Notify(uploadId, BatchProcessStatuses.ImportInProgress, rowsProcessed, totalRows, message, percentage);
        }

        private void downloadAndSaveImageLocally(string url, string pathForSavingFilesLocally, string modelNumber)
        {
            WebClient client = new WebClient();
            client.DownloadFileAsync(new Uri(url), pathForSavingFilesLocally + modelNumber + ".jpg");
        }
    }
}