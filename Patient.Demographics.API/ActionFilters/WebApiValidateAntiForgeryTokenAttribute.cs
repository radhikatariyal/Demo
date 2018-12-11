using Patient.Demographics.Web;
using System;
using System.Linq;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http.Filters;

namespace Patient.Demographics.API.ActionFilters
{
    public sealed class WebApiValidateAntiForgeryTokenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            if (actionContext.Request.Method.Method.ToUpper() != "GET" 
                && actionContext.Request.Method.Method.ToUpper() != "OPTIONS"
                && actionContext.RequestContext.Principal.Identity.IsAuthenticated)
            {
                AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;

                var headers = actionContext.Request.Headers;

                var tokenCookie = headers
                    .GetCookies()
                    .Select(c => c[Constants.Cookies.X_XSRF])
                    .FirstOrDefault();

                var tokenHeader = string.Empty;

                //if (headers.GetValues(Constants.Tokens.X_XSRF) != null)
                //{
                //    tokenHeader = headers.GetValues(Constants.Tokens.X_XSRF).FirstOrDefault();
                //    AntiForgery.Validate(tokenCookie != null ? tokenCookie.Value : null, tokenHeader);
                //}
            }

            base.OnActionExecuting(actionContext);
        }
    }
}
