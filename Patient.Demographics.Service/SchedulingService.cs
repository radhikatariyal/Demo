using System;
using Patient.Demographics.CrossCutting.Logger;
using Patient.Demographics.Service.Queues;
using Castle.MicroKernel;

namespace Patient.Demographics.Service
{
    public class SchedulingService : ISchedulingService
    {
        private readonly IKernel _container;
        private readonly ILogger _logger;

        public SchedulingService(
            IKernel container,
            ILogger logger
        )
        {
            _container = container;
            _logger = logger;
        }

        public void Start()
        {
            try
            {
                UploadStatusConsumer.Start(_container);
                //ProgramCataloguePublishConsumer.Start(_container);
            }

            catch (Exception exception)
            {
                _logger.LogExceptionAsync(exception);
                throw exception;
            }
        }

        public void Stop()
        {
        }
    }
}