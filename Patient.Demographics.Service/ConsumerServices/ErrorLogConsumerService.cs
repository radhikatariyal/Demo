using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Patient.Demographics.CrossCutting.Logger;
using MassTransit;
using Newtonsoft.Json;
using Patient.Demographics.Service.Common;

namespace Patient.Demographics.Service.ConsumerServices
{
    public interface IErrorLogConsumerService
    {
        Task LogGenericException(string eventName, IEnumerable<ExceptionInfo> exceptions, string messageJson);
    }

    public class ErrorLogConsumerService : IErrorLogConsumerService
    {
        private readonly ILogger _logger;

        public ErrorLogConsumerService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task LogGenericException(string eventName, IEnumerable<ExceptionInfo> exceptions, string messageJson)
        {
            foreach (var exception in exceptions)
            {
                var errorMessage = $"Error: {exception.Message} - Stack Trace: {exception.StackTrace} - Source: {exception.Source} - Message: {messageJson}";
                DebugConsole.WriteLine($"Exception received from {eventName} with the details: {errorMessage}");
                await _logger.LogExceptionAsync(new Exception(errorMessage));
            }
        }
    }
}
