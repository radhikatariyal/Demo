using System.Collections.Generic;

namespace Patient.Demographics.Service.FileUploads.Validators.ValidationChecks
{
    public class RequiredFieldCheck : IColumnValidationCheck
    {
        public void PerformCheck(ICollection<string> errors, string sourceColumnName, string field, string identifierField = null)
        {
            if (string.IsNullOrEmpty(field))
            {
                errors.Add($"Required field '{sourceColumnName}' is missing");
            }
        }
    }
}