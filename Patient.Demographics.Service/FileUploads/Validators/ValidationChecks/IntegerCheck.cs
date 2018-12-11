using System.Collections.Generic;
using System.Globalization;

namespace Patient.Demographics.Service.FileUploads.Validators.ValidationChecks
{
    public class IntegerCheck : IColumnValidationCheck
    {
        public void PerformCheck(ICollection<string> errors, string sourceColumnName, string field, string identifierField = null)
        {
            if (string.IsNullOrEmpty(field))
            {
                return;
            }

            int value;
            var numberIsInteger = int.TryParse(field, NumberStyles.Any, null, out value);

            if (!numberIsInteger)
            {
                errors.Add($"Integer field '{sourceColumnName}' has an invalid value");
            }
        }
    }
}