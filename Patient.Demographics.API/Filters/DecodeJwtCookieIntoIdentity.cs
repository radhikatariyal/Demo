using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using AuthorizationContext = System.Web.Mvc.AuthorizationContext;

namespace Patient.Demographics.Web.Filters
{
    public sealed class DecodeJwtCookieIntoIdentity : AuthorizeAttribute
    {
        public static readonly int ExecutionOrder = 1;
        public DecodeJwtCookieIntoIdentity()
        {
            Order = ExecutionOrder;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var accessToken = filterContext.RequestContext.HttpContext.Request.Cookies[Constants.Cookies.ACCESSTOKEN];

            if (!string.IsNullOrEmpty(accessToken?.Value))
            {
                var handler = new JwtSecurityTokenHandler();
                var result = (JwtSecurityToken)handler.ReadToken(accessToken.Value);
                var claims = (List<Claim>)result.Claims;
                var userName = claims[1].Value;

                var identity = new Identity
                {
                    Username = userName
                };

                var roles = claims.Where(r => r.Type == "role");

                var principal = new Principal(identity, roles.Select(r => r.Value).ToArray());

                filterContext.RequestContext.HttpContext.User = principal;
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return true;
        }
    }
}