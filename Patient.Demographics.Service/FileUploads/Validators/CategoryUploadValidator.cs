using Patient.Demographics.Service.FileUploads;
using Patient.Demographics.Service.FileUploads.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Patient.Demographics.Common;
using Patient.Demographics.Repository.Repositories;
using Patient.Demographics.Repository.Dtos;
using Patient.Demographics.Commands;
using Patient.Demographics.CrossCutting.Logger;
using Patient.Demographics.Service.Exceptions;
using Patient.Demographics.Service.FileUploads.Validators.ValidationChecks;
using Patient.Demographics.Data;
using Patient.Demographics.Repositories.Repositories;
using static Patient.Demographics.Common.Enums;

namespace Patient.Demographics.Service.FileUploads.Validators
{
    public class CategoryUploadValidator : UploadValidatorBase, IUploadValidator
    {
        private readonly IBatchProcessMasterRepository _masterRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISupplierRepository _supplierRepository;

        public CategoryUploadValidator(
            ICommandExecutor commandExecutor
            , IUploadsNotifier uploadsNotifier
            , ILogger logger
            , ISqlDataContextFactory dataContextFactory
            , IBatchProcessMasterRepository masterRepository
            , ISqlDataContext dbContext
            , ICategoryRepository categoryRepository
            , ISupplierRepository supplierRepository

            )
            : base(commandExecutor, uploadsNotifier, logger, dataContextFactory, dbContext, categoryRepository, supplierRepository)
        {
            _masterRepository = masterRepository;
            _categoryRepository = categoryRepository;
            _supplierRepository = supplierRepository;
        }

        protected override async Task<BatchProcessSummaryDto> ValidateBusinessRule(ISqlDataContext dataContext, BatchProcessDto upload, IList<string> items, IList<string> columnHeaders, Dictionary<string, ErrorCodes> errors, Guid updatedByUserId, object obj)
        {
            var peopleSoftCategories = obj as List<CategoryPeopleSoftList>;
            var category1 = items[0];
            var category2 = items[1];
            var category3 = items[2];
            var category4 = items[3];
            var isBiwCatExists = peopleSoftCategories.Where(x => x.BiwCategory1 == category1 && x.BiwCategory2 == category2 && x.BiwCategory3 == category3 && x.BiwCategory4 == category4).FirstOrDefault();

            if (isBiwCatExists == null)
            {
                errors.Add($"category combination {category1} ==> {category2} ==> {category3} ==> {category4} is not a BIW Category",ErrorCodes.NOTBIWCategory);
            }

            if (string.IsNullOrWhiteSpace(items[5]) && string.IsNullOrWhiteSpace(items[6]) && string.IsNullOrWhiteSpace(items[7]) && string.IsNullOrWhiteSpace(items[8]) && string.IsNullOrWhiteSpace(items[9]) && string.IsNullOrWhiteSpace(items[10]))
            {
                errors.Add($"supplier category combination cannot be null or empty", ErrorCodes.BIWCategotycombinationEmpty);
            }

            var isNew = false;
            var categoryExists = dataContext.Categories.Where(row => row.BiwCategory1 == category1 && row.BiwCategory2 == category2 && row.BiwCategory3 == category3 && row.BiwCategory4 == category4).FirstOrDefault();
            if (categoryExists == null)
            {
                isNew = true;
            }

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

        public BatchProcessTypes Handles => BatchProcessTypes.FltCategory;

        protected override async Task<BatchProcessMasterDto> GetMasterDto(BatchProcessDto upload)
        {
            return await _masterRepository.GetBatchProcessMasterAsync(upload.ContextId);
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
            return await _masterRepository.GetCategoryRules(upload);
        }
    }
}
