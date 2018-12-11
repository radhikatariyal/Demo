using System;
using System.Collections.Generic;
using System.Linq;

namespace Patient.Demographics.Service.FileUploads.Validators.ValidationChecks
{
    public class DuplicateFieldValueCheck : IColumnValidationCheck
    {
        private readonly List<FieldValue> _knownValues;

        public DuplicateFieldValueCheck(List<FieldValue> initialValues)
        {
            _knownValues = initialValues;
        }

        public void PerformCheck(ICollection<string> errors, string sourceColumnName, string field, string identifierField)
        {
            if (_knownValues.Any(s => s.Field.Equals(field, StringComparison.OrdinalIgnoreCase) && !s.Identifier.Equals(identifierField, StringComparison.OrdinalIgnoreCase)))
            {
                errors.Add($"'{field}' already exist in '{sourceColumnName}'");
            }

            _knownValues.Add(new FieldValue(identifierField, field));
        }
    }
}