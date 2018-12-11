using System.Collections.Generic;
using System.Globalization;

namespace Patient.Demographics.Service.FileUploads.Validators.ValidationChecks
{
    public class DecimalCheck : IColumnValidationCheck
    {
        public void PerformCheck(ICollection<string> errors, string sourceColumnName, string field, string identifierField = null)
        {
            if (string.IsNullOrEmpty(field))
            {
                return;
            }

            decimal value;
            var numberIsDecimal = decimal.TryParse(field, NumberStyles.Any, null, out value);

            if (!numberIsDecimal)
            {
                errors.Add($"Decimal field '{sourceColumnName}' has an invalid value");
            }
        }
    }
}