using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Patient.Demographics.API.ActionFilters
{
    /// <summary>
    /// Hooks into EF calls for each requet and stores the queries and stats in miniProfiler tables.
    /// It is possible to capture more fine grained information using the MiniProfiler API
    /// http://miniprofiler.com/
    /// </summary>
    public class ProfilingActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            WebApiProfiler.Start();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            WebApiProfiler.Stop();
        }
    }
}