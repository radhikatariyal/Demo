using System;
using System.Collections.Generic;

namespace Patient.Demographics.Service.FileUploads.Validators.ValidationChecks
{
    public class FieldValueCheck : IColumnValidationCheck
    {
        private readonly HashSet<string> _items;

        public FieldValueCheck(IEnumerable<string> items)
        {
            _items = new HashSet<string>(items, StringComparer.OrdinalIgnoreCase);
        }

        public void PerformCheck(ICollection<string> errors, string sourceColumnName, string field, string identifier = null)
        {
            if (string.IsNullOrEmpty(field))
            {
                return;
            }

            if (!_items.Contains(field))
            {
                errors.Add($"'{field}' does not exist in '{sourceColumnName}'");
            }
        }
    }
}