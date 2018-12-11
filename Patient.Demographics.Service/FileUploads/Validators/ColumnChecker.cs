using System.Collections.Generic;
using Patient.Demographics.Service.FileUploads.Validators.ValidationChecks;

namespace Patient.Demographics.Service.FileUploads.Validators
{
    public class ColumnChecker
    {
        public ColumnChecker(string sourceColumnName)
        {
            SourceColumnName = sourceColumnName;
            _validators = new List<IColumnValidationCheck>();
        }

        private readonly List<IColumnValidationCheck> _validators;

        private DuplicateExternalIdentifierCheck _duplicateExternalIdentityChecker = null;

        private string _externalIdentifier = null;

        public string SourceColumnName { get; }

        public void AddValidator(IColumnValidationCheck columnValidationCheck)
        {
            _validators.Add(columnValidationCheck);
        }

        public void AddDuplicateExternalIdentityChecker(DuplicateExternalIdentifierCheck duplicateExternalIdentifierCheck)
        {
            _duplicateExternalIdentityChecker = duplicateExternalIdentifierCheck;
        }

        public ICollection<string> PerformChecks(ICollection<string> errors, string field, string identityField = null)
        {
            foreach (var validator in _validators)
            {
                validator.PerformCheck(errors, SourceColumnName, field, identityField);
            }

            if (_duplicateExternalIdentityChecker != null)
            {
                if (!string.IsNullOrEmpty(field.Trim()))
                {
                    _externalIdentifier = _duplicateExternalIdentityChecker.PerformCheck(errors, SourceColumnName, field);
                }
            }

            return errors;
        }

        public string ExternalIdentifier => _externalIdentifier;

        public bool HasDuplicateExternalIdentityChecker => _duplicateExternalIdentityChecker != null;
    }
}