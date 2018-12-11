using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Patient.Demographics.CrossCutting.Logger;

namespace Patient.Demographics.API.ActionFilters
{
    public class ExceptionActionFilter : ActionFilterAttribute, IExceptionFilter
    {
        private readonly ILogger _logger;

        public ExceptionActionFilter(ILogger logger)
        {
            _logger = logger;
        }

        public async Task ExecuteExceptionFilterAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            if (actionExecutedContext != null && actionExecutedContext.Exception != null)
            {
                await _logger.LogExceptionAsync(actionExecutedContext.Exception);
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}