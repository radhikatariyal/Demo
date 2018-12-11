using System.Net.Http.Headers;
using System.Web.Http;
using Patient.Demographics.CrossCutting.Logger;
using Patient.Demographics.API.ActionFilters;
using Bootstrap.Extensions.StartupTasks;

namespace Patient.Demographics.API.Configuration
{
    public class RoutingTask : IStartupTask
    {
        private readonly ILogger _logger;

        public RoutingTask(ILogger logger)
        {
            _logger = logger;
        }

        public void Run()
        {
            GlobalConfiguration.Configure(x => x.MapHttpAttributeRoutes());
            Register(GlobalConfiguration.Configuration);
        }

        public void Reset()
        {

        }

        public void Register(HttpConfiguration config)
        {
            // Web API routes and json configuration
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.Filters.Add(new ExceptionActionFilter(_logger));
            config.Filters.Add(new CommandBindingActionFilter());
            //config.Filters.Add(new WebApiValidateAntiForgeryTokenAttribute());
            config.Filters.Add(new SanitizeHtmlInput());
        }
    }
}