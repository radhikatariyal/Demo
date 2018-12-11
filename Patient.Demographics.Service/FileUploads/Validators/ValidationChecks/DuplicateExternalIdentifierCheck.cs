using System;
using System.Collections.Generic;

namespace Patient.Demographics.Service.FileUploads.Validators.ValidationChecks
{
    public class DuplicateExternalIdentifierCheck
    {
        private readonly HashSet<string> _incomingIdentifiers;

        public DuplicateExternalIdentifierCheck()
        {
            _incomingIdentifiers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public string PerformCheck(ICollection<string> errors, string sourceColumnName, string field)
        {
            if (_incomingIdentifiers.Contains(field))
            {
                errors.Add($"Duplicate key in '{sourceColumnName}'");
                return null;
            }

            _incomingIdentifiers.Add(field);
            return field;
        }
    }
}