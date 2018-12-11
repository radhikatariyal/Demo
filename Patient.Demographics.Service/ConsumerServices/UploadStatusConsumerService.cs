using Patient.Demographics.Common;
using Patient.Demographics.Events.BatchProces;
using Patient.Demographics.Service.FileUploads;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patient.Demographics.Service.ConsumerServices
{
    public interface IUploadStatusConsumerService
    {
        Task Run(BatchProcessStatusChangedEvent @event);
    }

    public class UploadStatusConsumerService : IUploadStatusConsumerService
    {
        private readonly IList<IUploadStatusChangeQueue> _queues;

        public UploadStatusConsumerService(IList<IUploadStatusChangeQueue> queues)
        {
            _queues = queues;
        }

        public async Task Run(BatchProcessStatusChangedEvent @event)
        {
            if (@event.StatusAfter == BatchProcessStatuses.ReadyToImport ||
                @event.StatusAfter == BatchProcessStatuses.ValidationInProgress)
            {
                var queue = _queues.First(q => q.UploadStatus == @event.StatusAfter);
                await queue.Complete(@event);
            }
        }
    }
}