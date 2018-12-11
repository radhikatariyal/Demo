using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Configuration;

namespace Patient.Demographics.API.Helpers
{
    public static class GPSMAntiForgeryToken
    {
        public static IHtmlString GenerateGPSMAntiForgeryToken(this HtmlHelper helper)
        {
            return MvcHtmlString.Create(string.Format("<input type=\"hidden\" value=\"{0}\" name=\"__GPSMRequestVerificationToken\">", GetTokenHeaderValue()));
        }

        private static string GetTokenHeaderValue()
        {
            string cookieToken, formToken;      

            AntiForgery.GetTokens(null, out cookieToken, out formToken);

            var cookie = new HttpCookie("X-XSRF-Cookie");

            cookie.HttpOnly = true;
            cookie.Domain = ConfigurationManager.AppSettings["domains"];
            cookie.Value = cookieToken;

            HttpContext.Current.Response.Cookies.Add(cookie);

            return formToken;
        }
    }
}