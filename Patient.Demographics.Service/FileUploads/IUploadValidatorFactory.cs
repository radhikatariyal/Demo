using Patient.Demographics.Common;

namespace Patient.Demographics.Service.FileUploads
{
    public interface IUploadValidatorFactory
    {
        IUploadValidator GetValidator(BatchProcessTypes uploadType);
    }
};