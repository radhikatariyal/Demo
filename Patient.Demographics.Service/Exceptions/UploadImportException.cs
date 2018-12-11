using System;

namespace Patient.Demographics.Service.Exceptions
{
    public class UploadImportException : ApplicationException
    {
        public UploadImportException()
        {

        }

        public UploadImportException(string uploadId, string errors) : base($"error importing upload {uploadId}, {errors}")
        {

        }
    }
}