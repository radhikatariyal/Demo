using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Patient.Demographics.Common;
using Patient.Demographics.Repository.Repositories;
using Patient.Demographics.Repository.Dtos;
using Patient.Demographics.Commands;
using Patient.Demographics.CrossCutting.Logger;
using Patient.Demographics.Data;
using Patient.Demographics.Service.Exceptions;
using Patient.Demographics.Service.FileUploads.Validators.ValidationChecks;
using Patient.Demographics.Repositories.Repositories;
using static Patient.Demographics.Common.Enums;

namespace Patient.Demographics.Service.FileUploads.Validators
{
    public class FltProductUploadValidator : UploadValidatorBase, IUploadValidator
    {
        private readonly IBatchProcessMasterRepository _masterRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISupplierRepository _supplierRepository;

        public FltProductUploadValidator(
            ICommandExecutor commandExecutor
            , IUploadsNotifier uploadsNotifier
            , ILogger logger
            , ISqlDataContextFactory dataContextFactory
            , IBatchProcessMasterRepository masterRepository
            , ISqlDataContext dbContext
            , ICategoryRepository categoryRepository
            , ISupplierRepository supplierRepository)
            : base(commandExecutor, uploadsNotifier, logger, dataContextFactory, dbContext, categoryRepository, supplierRepository)
        {
            _masterRepository = masterRepository;
            _categoryRepository = categoryRepository;
            _supplierRepository = supplierRepository;
        }

        protected override async Task<BatchProcessSummaryDto> ValidateBusinessRule(ISqlDataContext dataContext, BatchProcessDto upload, IList<string> items, IList<string> columnHeaders, Dictionary<string, ErrorCodes> errors, Guid updatedByUserId, object obj)
        {
            var category1 = items[5];
            var category2 = items[6];
            var category3 = items[7];
            string modelNumber = Convert.ToString(items[3]);
            
            // var isCategoryExists = dataContext.FltCategories.Where(row => row.Category1 == category1 && row.Category2 == category2 && row.Category3 == category3).FirstOrDefault();

            //if (isCategoryExists == null)
            //   errors.Add($"category combination {category1} ==> {category2} ==> {category3} is missing");

            // var isProductExists = dataContext.Products.Where(p => p.ModelNumber == modelNumber).FirstOrDefault();

            var isNew = false;

           // if (isProductExists == null) isNew = true;

            return new BatchProcessSummaryDto
            {
                Errors = errors,
                RecordsToAdd = isNew ? 1 : 0,
                RecordsToUpdate = isNew ? 0 : 1,
            };
        }

        protected override async Task<Dictionary<ColumnChecker, int>> GetIndexedColumnCheckers(BatchProcessDto upload, IList<LiteralColumnMappingDto> columnMappings, IList<HierarchyColumnMappingDto> hierarchyColumnMappings, string externalIdentifierColumnName, Guid updatedByUserId, IList<string> columnHeadings2)
        {
            var errors = new List<string>();

            var allMappings = new List<ColumnMappingBaseDto>();
            allMappings.AddRange(columnMappings);

            var positions = new Dictionary<ColumnChecker, int>();

            foreach (var mapping in allMappings)
            {
                var columnIndex = GetColumnIndex(columnHeadings2, mapping);

                if (columnIndex == -1 && mapping.Required)
                {
                    errors.Add($"Required column {mapping.SourceColumn} is missing");
                }

                var columnChecker = new ColumnChecker(mapping.SourceColumn);

                if (mapping.Required)
                {
                    columnChecker.AddValidator(new RequiredFieldCheck());
                }

                var simple = mapping as LiteralColumnMappingDto;

                if (simple != null)
                {
                    AddCommonValidators(externalIdentifierColumnName, simple, columnChecker);
                }

                positions.Add(columnChecker, columnIndex);

            }

            if (errors.Any())
            {
                var joinedErrors = string.Join("; ", errors);
                await UpdateUploadWithErrorsAsync(upload.Id, joinedErrors,"", updatedByUserId);

                throw new UploadValidationException(joinedErrors);
            }

            return positions;
        }

        public BatchProcessTypes Handles => BatchProcessTypes.FltProduct;

        protected override async Task<BatchProcessMasterDto> GetMasterDto(BatchProcessDto upload)
        {
            string id = upload.ContextId.ToString().ToUpper();
            return await _masterRepository.GetBatchProcessMasterAsync(Guid.Parse(id));
        }

        protected override async Task<IReadOnlyCollection<string>> GetExistingExternalIdentifiers(BatchProcessDto upload)
        {
            return null;
        }

        protected override async Task<IList<IRowValidationCheck>> GetRowValidationChecks()
        {
            return null;
        }

        protected override async Task<IList<BatchProductRulesDto>> GetRules(BatchProcessDto upload)
        {            
            return await _masterRepository.GetProductRule(upload);
        }
       
    }


}
