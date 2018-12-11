using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Patient.Demographics.Commands;
using Microsoft.AspNet.Identity;

namespace Patient.Demographics.API.ActionFilters
{
    public class CommandBindingActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            var commandKvp = filterContext.ActionArguments.FirstOrDefault(c => c.Value is Command);

            if (commandKvp.Value == null)
                return;

            var command = (Command)commandKvp.Value;
            var user = filterContext.RequestContext.Principal;

            if (user == null || !user.Identity.IsAuthenticated)
                return;

            var impersonationClaim = ((ClaimsIdentity)user.Identity).Claims.SingleOrDefault(c => c.Type == Web.Constants.Claims.IMPERSONATOR_ID);

            command.CommandIssuedByUserId = Guid.Parse(impersonationClaim != null
                ? impersonationClaim.Value
                : user.Identity.GetUserId());
        }
    }
}