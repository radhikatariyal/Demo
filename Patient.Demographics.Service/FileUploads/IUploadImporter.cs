using System;
using System.Threading.Tasks;

namespace Patient.Demographics.Service.FileUploads
{
    public interface IUploadImporter : IHandleUpload
    {
        Task Import(Guid uploadId, Guid updatedByUserId);
    }
}