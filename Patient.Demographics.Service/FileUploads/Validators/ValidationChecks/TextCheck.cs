using System.Collections.Generic;

namespace Patient.Demographics.Service.FileUploads.Validators.ValidationChecks
{
    public class TextCheck : IColumnValidationCheck
    {
        public const int MaximumLengthForTextField = 200;

        public void PerformCheck(ICollection<string> errors, string sourceColumnName, string field, string identifierField = null)
        {
            if (field.Length > MaximumLengthForTextField)
            {
                errors.Add($"Field '{sourceColumnName}' is longer than {MaximumLengthForTextField} characters");
            }
        }
    }
}