using System;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Patient.Demographics.API.Controllers
{
    public class BaseApiController : ApiController
    {
        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

       

        protected IHttpActionResult CreatedResult(Uri location)
        {
            return new CreatedResult(location, Request);
        }

        protected IHttpActionResult CreatedResult()
        {
            return new CreatedResult(null, Request);
        }

        protected IHttpActionResult NoContentResult()
        {
            return new NoContentResult(Request);
        }

        protected Uri CreatedUri(string linkName, string id, string parameterName = "id")
        {
            var routerValues = new Dictionary<string, object> { { parameterName, id }};
            var locationHeader = new Uri(Url.Link(linkName, routerValues));

            return locationHeader;
        }

        protected Guid CurrentUserId()
        {
            Guid currentUserId = Guid.Empty;
            if (User != null && User.Identity.IsAuthenticated)
            {
                currentUserId = Guid.Parse(User.Identity.GetUserId());
            }
            else
            {
                ModelState.AddModelError(string.Empty, "There is no currently logged in user");
               
            }
            return currentUserId;
        }
       
    }
}