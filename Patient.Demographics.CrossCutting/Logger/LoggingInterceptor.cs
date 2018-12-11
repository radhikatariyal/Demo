using Castle.DynamicProxy;
using System;
using System.Threading.Tasks;

namespace Patient.Demographics.CrossCutting.Logger
{
    public class LoggingInterceptor : IInterceptor
    {
        private readonly ILogger _logger;

        public LoggingInterceptor(ILogger logger)
        {
            _logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();

                var returnedTask = invocation.ReturnValue as Task;
                if (returnedTask != null && returnedTask.IsFaulted && returnedTask.Exception != null)
                {
                    foreach (var innerException in returnedTask.Exception.InnerExceptions)
                    {
                        _logger.LogException(innerException);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                throw;
            }
        }
    }
}