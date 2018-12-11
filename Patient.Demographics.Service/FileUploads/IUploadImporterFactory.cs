using Patient.Demographics.Common;

namespace Patient.Demographics.Service.FileUploads
{
    public interface IUploadImporterFactory
    {
        IUploadImporter GetImporter(BatchProcessTypes uploadType);
    }
}