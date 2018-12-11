using Patient.Demographics.Events;
using Patient.Demographics.Events.BatchProces;
using Patient.Demographics.Service.Common;
using Patient.Demographics.Service.FileUploads;
using System.Threading.Tasks;

namespace Patient.Demographics.Service.EventHandlers
{
    public class UploadNotificationEventHandler : IEventHandler<BatchProcessNotificationEvent>
    {

        private readonly IUploadsNotifier _uploadsNotifier;

        public UploadNotificationEventHandler(IUploadsNotifier uploadsNotifier)
        {
            _uploadsNotifier = uploadsNotifier;
        }

        public async Task HandleAsync(BatchProcessNotificationEvent @event)
        {
            DebugConsole.WriteLine($"Sending Import Status Update {@event.NumberOfRowsProcessed}/{@event.TotalNumberOfRows}");
            await _uploadsNotifier.Notify(@event.UploadId, @event.UploadStatus, @event.NumberOfRowsProcessed, @event.TotalNumberOfRows, "", @event.Percentage);
        }
    }
}