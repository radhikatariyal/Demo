using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Patient.Demographics.Service.FileUploads.Validators.ValidationChecks
{
    public class EmailCheck : IColumnValidationCheck
    {
        public const int MaximumLengthForTextField = 200;

        public void PerformCheck(ICollection<string> errors, string sourceColumnName, string field, string identifierField = null)
        {
            if (string.IsNullOrEmpty(field))
            {
                return;
            }

            if (field.Length > MaximumLengthForTextField)
            {
                errors.Add($"Field '{sourceColumnName}' is longer than {MaximumLengthForTextField} characters");
            }
            else
            {
                var isEmail = Regex.IsMatch(field, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

                if (!isEmail)
                {
                    errors.Add($"Field '{sourceColumnName}' is not in valid format");
                }
            }
        }
    }
}