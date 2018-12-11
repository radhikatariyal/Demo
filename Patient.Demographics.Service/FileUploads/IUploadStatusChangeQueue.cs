using System.Threading.Tasks;
using Patient.Demographics.Common;
using Patient.Demographics.Events.BatchProces;

namespace Patient.Demographics.Service.FileUploads
{
    public interface IUploadStatusChangeQueue
    {
        BatchProcessStatuses UploadStatus { get; }
        Task Complete(BatchProcessStatusChangedEvent @event);
    }
}