using System;
using System.Collections.Generic;
using System.Globalization;

namespace Patient.Demographics.Service.FileUploads.Validators.ValidationChecks
{
    public class DateCheck : IColumnValidationCheck
    {
        public void PerformCheck(ICollection<string> errors, string sourceColumnName, string field, string identifierField = null)
        {
            if (string.IsNullOrEmpty(field))
            {
                return;
            }

            var dateFormats = new[]
            {
                "d-M-yyyy",
                "d-MM-yyyy",
                "dd-M-yyyy",
                "dd-MM-yyyy",
                "d/M/yyyy",
                "d/MM/yyyy",
                "dd/M/yyyy",
                "dd/MM/yyyy"
            };

            DateTime value;
            var numberIsDate = DateTime.TryParseExact(field, dateFormats, null, DateTimeStyles.NoCurrentDateDefault, out value);

            if (!numberIsDate)
            {
                errors.Add($"Date field '{sourceColumnName}' has an invalid value");
            }
        }
    }
}