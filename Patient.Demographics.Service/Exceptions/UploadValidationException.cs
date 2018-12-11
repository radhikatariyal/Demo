using System;

namespace Patient.Demographics.Service.Exceptions
{
    public class UploadValidationException : ApplicationException
    {
        public UploadValidationException()
        {
            
        }

        public UploadValidationException(string errors) : base($"Error validating upload - {errors}")
        { 
            
        }
    }
}
