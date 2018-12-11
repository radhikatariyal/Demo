using System.Collections.Generic;

namespace Patient.Demographics.Service.FileUploads.Validators.ValidationChecks
{
    public interface IColumnValidationCheck
    {
        void PerformCheck(ICollection<string> errors, string sourceColumnName, string field,
            string identifierField = null);
    }   
}
