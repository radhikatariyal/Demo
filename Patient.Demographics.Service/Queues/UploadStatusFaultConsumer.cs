using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Patient.Demographics.Common.Settings;
using Patient.Demographics.CrossCutting.MessageQueue;
using Patient.Demographics.Events.BatchProces;
using Patient.Demographics.Service.ConsumerServices;
using Castle.MicroKernel;
using MassTransit;
using Newtonsoft.Json;

namespace Patient.Demographics.Service.Queues
{
    public class UploadStatusFaultConsumer : IConsumer<Fault<BatchProcessStatusChangedEvent>>
    {
        private readonly IErrorLogConsumerService _errorLogConsumerService;

        public UploadStatusFaultConsumer(IErrorLogConsumerService errorLogConsumerService)
        {
            _errorLogConsumerService = errorLogConsumerService;
        }

        public static void Start(IKernel container)
        {
            var config = container.Resolve<IMessageQueueSettings>();
            var consumer = container.Resolve<UploadStatusFaultConsumer>();
            QueueManager.Configure(consumer, "upload_status_update_queue_error", config.MessageQueueUsername, config.MessageQueuePassword, config.MessageQueueVHost, null, Retry.Exponential(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1)));
        }

        public async Task Consume(ConsumeContext<Fault<BatchProcessStatusChangedEvent>> context)
        {
            await _errorLogConsumerService.LogGenericException(nameof(BatchProcessStatusChangedEvent), context.Message.Exceptions, JsonConvert.SerializeObject(context.Message));
        }
    }
}
