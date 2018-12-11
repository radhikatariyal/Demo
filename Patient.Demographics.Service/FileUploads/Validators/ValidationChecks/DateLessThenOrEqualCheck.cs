using Patient.Demographics.Common;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Patient.Demographics.Service.FileUploads.Validators.ValidationChecks
{
    public class DateLessThenOrEqualCheck : IColumnValidationCheck
    {
        public void PerformCheck(ICollection<string> errors, string sourceColumnName, string field, string identifierField = null)
        {
            if (string.IsNullOrEmpty(field))
            {
                return;
            }

            DateTime value;
            if (DateTime.TryParseExact(field, DateTimeFormats.All, null, DateTimeStyles.NoCurrentDateDefault, out value))
            {
                if (value > DateTime.Today)
                {
                    errors.Add($"Date field '{sourceColumnName}' can not be greater than current date");
                }
            }
        }
    }
}