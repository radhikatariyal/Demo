using System;
using System.Threading.Tasks;
using Patient.Demographics.Common;
using Patient.Demographics.Repository.Dtos;

namespace Patient.Demographics.Service.FileUploads
{
    public interface IUploadsNotifier
    {
        Task Notify(Guid uploadId, BatchProcessStatuses status, string message, int percentage);
        Task Notify(Guid uploadId, BatchProcessStatuses status, int rowsProcessed, int totalRows, string message, int percentage);
        Task Notify(Guid uploadId, BatchProcessStatuses status, BatchProcessValidationResultSummaryDto validationResults, int percentage);
    }
}
