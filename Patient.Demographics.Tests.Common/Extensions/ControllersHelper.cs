using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using NSubstitute;

namespace Patient.Demographics.Tests.Common
{
    public static class ControllersHelper
    {
        public const string BaseUrl = "http://web.patient.localhost";

        public static HttpControllerContext CreateControllerContext(string requestUri = "http://api.gpsm.localhost")
        {
            var config = Substitute.For<HttpConfiguration>();
            var routeData = Substitute.For<IHttpRouteData>();
            var request = Substitute.For<HttpRequestMessage>();
            request.RequestUri = new Uri(requestUri);
            return new HttpControllerContext(config, routeData, request);
        }

        public static bool IsDecoratedWith<T>(ApiController controller)
        {
            return controller
            .GetType()
            .GetCustomAttributes(typeof(T), true)
            .Any();
        }

        public static bool IsDecoratedWith<T>(ApiController controller, string methodName)
        {
            var method = controller.GetType().GetMethod(methodName);
            var hasAttribute =
                method.GetCustomAttributes(typeof(T), false).Any();
            return hasAttribute;
        }

        public static ApiController AddBaseUrlLink(this ApiController controller)
        {
            if (controller.Url == null)
            {
                controller.Url = Substitute.For<UrlHelper>();
            }

            controller.Url.Link(Arg.Any<string>(), Arg.Any<object>())
                     .Returns(BaseUrl);

            return controller;
        }

        public static ApiController AddIdUrlLink(this ApiController controller, string linkName, string id, string url, string parameter = "id")
        {
            if (controller.Url == null)
            {
                controller.Url = Substitute.For<UrlHelper>();
            }

            controller.Url.Link(linkName, Arg.Is<Dictionary<string, object>>(d => d[parameter].ToString() == id.ToString()))
                .Returns(url);

            return controller;
        }

        public static ApiController AddIdUrlLink(this ApiController controller, string linkName, Guid id, string url)
        {
            return AddIdUrlLink(controller, linkName, id.ToString(), url);
        }

        public static ApiController SetUserId(this ApiController controller, Guid userId)
        {
            var claim = new Claim("mockClaim", userId.ToString());
            var user = Substitute.For<IPrincipal>();
            var identity = Substitute.For<ClaimsIdentity>();
            identity.FindFirst(Arg.Any<string>()).Returns(claim);
            identity.IsAuthenticated.Returns(true);
            user.Identity.Returns(identity);
            controller.User = user;
            return controller;
        }
    }
}