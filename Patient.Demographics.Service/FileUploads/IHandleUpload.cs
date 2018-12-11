using Patient.Demographics.Common;

namespace Patient.Demographics.Service.FileUploads
{
    public interface IHandleUpload
    {
        BatchProcessTypes Handles { get; }
    }
}