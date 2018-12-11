using System;
using System.Threading.Tasks;
using Patient.Demographics.Repository.Dtos;

namespace Patient.Demographics.Service.FileUploads
{
    public interface IUploadValidator : IHandleUpload
    {
        Task ValidateAsync(BatchProcessDto upload, Guid updatedByUserId);
    }
}