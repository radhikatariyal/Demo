using Patient.Demographics.Events.BatchProces;
using Castle.MicroKernel;
using MassTransit;
using System.Threading.Tasks;
using Patient.Demographics.Common.Settings;
using Patient.Demographics.CrossCutting.MessageQueue;
using Patient.Demographics.Service.ConsumerServices;

namespace Patient.Demographics.Service.Queues
{
    public class UploadStatusConsumer : IConsumer<BatchProcessStatusChangedEvent>
    {
        private readonly IUploadStatusConsumerService _uploadStatusConsumerService;

        public static void Start(IKernel container)
        {
            var config = container.Resolve<IMessageQueueSettings>();
            var consumer = container.Resolve<UploadStatusConsumer>();
            QueueManager.Configure(consumer, "upload_status_update_queue", config.MessageQueueUsername, config.MessageQueuePassword, config.MessageQueueVHost);
        }

        public UploadStatusConsumer(IUploadStatusConsumerService uploadStatusConsumerService)
        {
            _uploadStatusConsumerService = uploadStatusConsumerService;
        }

        public async Task Consume(ConsumeContext<BatchProcessStatusChangedEvent> context)
        {
            await _uploadStatusConsumerService.Run(context.Message);
        }
    }
}