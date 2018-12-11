using Patient.Demographics.Configuration.Extensions;
using Patient.Demographics.CrossCutting.Identity;
using Patient.Demographics.Data.Identity;
using Microsoft.Owin;
using Microsoft.Owin.BuilderProperties;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Optimization;
using Patient.Demographics.API.ActionFilters;
using Microsoft.Owin.Security.DataHandler;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

[assembly: OwinStartup(typeof(Patient.Demographics.API.Startup))]
namespace Patient.Demographics.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            bool profilingEnabled = false;
            if (string.Equals(ConfigurationManager.AppSettings["dev:ProfileEnabled"], "true",
                StringComparison.OrdinalIgnoreCase))
            {
                //Needs to be called before any EF initialisation
                WebApiProfiler.Initialise();
                profilingEnabled = true;
            }
            ApplicationManager.Initialize(UseAssembly.This());
            ConfigureOptionsRequest(app);
            ConfigureOAuth(app);

            ConfigureXMLSerialization();
            //  ConfigureSignalR(app);
            //   ConfigureMessagingQueue(app);
            //   ClearBundleTransforms();
            //GlobalConfiguration.Configuration.Formatters.Add(new FileMediaFormatter());

            GlobalConfiguration.Configuration.Formatters.XmlFormatter.MediaTypeMappings.Add(
      new QueryStringMapping("type", "xml", new MediaTypeHeaderValue("application/xml")));
            if (profilingEnabled)
            {
                GlobalConfiguration.Configuration.Filters.Add(new ProfilingActionFilter());
            }
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.UseXmlSerializer = true;
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.JsonFormatter);
           
        }

        private void ConfigureSignalR(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
           
        }

        private void ConfigureXMLSerialization()
        {

            var xml = GlobalConfiguration.Configuration.Formatters.XmlFormatter;
            xml.Indent = true;
        }

        private void ConfigureOptionsRequest(IAppBuilder app)
        {
            app.Use(ConfigureOptionsRequestHandler);
        }

        //   [DebuggerNonUserCode]
        private Task ConfigureOptionsRequestHandler(IOwinContext context, Func<Task> next)
        {
            context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Content-Type, Location, Authorization, X-XSRF-Token, AdminId" });
            context.Response.Headers.Add("Access-Control-Expose-Headers", new[] { "Location, Content-Disposition, Password-Expired" });
            context.Response.Headers.Add("Access-Control-Allow-Credentials", new[] { "true" });

            var acceptableDomains = ConfigurationManager.AppSettings["acceptableDomains"].Split(',');
            var optionHeader =
                  context.Request.Headers.SingleOrDefault(
                      x => string.Equals(x.Key, "Origin", StringComparison.OrdinalIgnoreCase));

            if (optionHeader.Value != null)
            {
                var allowOrigin = Array.FindAll(acceptableDomains,
                    x => x.Equals(optionHeader.Value.First()));

                context.Response.Headers.Add("Access-Control-Allow-Origin", allowOrigin);
            }
            context.Response.ContentType = "text/xml"; //Must be 'text/xml'

            if (optionHeader.Value != null && context.Request.Method == "OPTIONS")
            {
                context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "POST, GET, PUT, DELETE, OPTIONS" });
                context.Response.Headers.Add("Access-Control-Max-Age", new[] { "1728000" });
                context.Response.StatusCode = 200;
                context.Response.ContentType = "text/xml"; //Must be 'text/xml'

                return context.Response.WriteAsync("handled");
            }
                   return next.Invoke();
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            var issuer = ConfigurationManager.AppSettings["as:Issuer"];
            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            double accessTokenLifetimeMinutes, refreshtokenLifetimeMinutes;
            if (!double.TryParse(ConfigurationManager.AppSettings["as:AuthenticationExpirationMinutes"],
                    out accessTokenLifetimeMinutes))
            {
                throw new ConfigurationErrorsException("Appsettings is missing as:AuthenticationExpirationMinutes key. Please set the JWT lifespan in minutes.");
            }
            if (!double.TryParse(ConfigurationManager.AppSettings["as:RefreshTokenLifetimeMinutes"],
                    out refreshtokenLifetimeMinutes))
            {
                throw new ConfigurationErrorsException("Appsettings is missing as:RefreshTokenLifetimeMinutes key. Please set the Refresh token lifespan in minutes.");
            }

            //app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            //{
            //    AuthenticationMode = AuthenticationMode.Active,
            //    AuthenticationType = "JWT",
            //    AllowInsecureHttp = true,
            //    TokenEndpointPath = new PathString("/oauth/token"),
            //    AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(accessTokenLifetimeMinutes),
            //  //  Provider = new JwtAuthorizationServerProvider(GlobalConfiguration.Configuration.DependencyResolver),
            //    AccessTokenFormat = new JwtFormatter(issuer, audienceId, ConfigurationManager.AppSettings["as:AudienceSecret"]),
            //    RefreshTokenProvider = new RefreshTokenProvider(refreshtokenLifetimeMinutes,
            //        () =>
            //            GlobalConfiguration.Configuration.DependencyResolver
            //                .GetService(typeof(IRefreshTokenService)) as IRefreshTokenService),
            //    RefreshTokenFormat = new TicketDataFormat(GlobalConfiguration.Configuration.DependencyResolver
            //                .GetService(typeof(IRefreshtokenDataProtector)) as IRefreshtokenDataProtector)
            //});

            //app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
            //{
            //    AuthenticationMode = AuthenticationMode.Active,
            //    AccessTokenFormat = new JwtFormatter(issuer, audienceId, ConfigurationManager.AppSettings["as:AudienceSecret"])
            //});
        }

        private void ConfigureMessagingQueue(IAppBuilder app)
        {
            //MassTransitMessageQueue.Start(
            //    ConfigurationManager.AppSettings["rmq:Username"],
            //    ConfigurationManager.AppSettings["rmq:Password"],
            //    ConfigurationManager.AppSettings["rmq:VHost"]);

            //var properties = new AppProperties(app.Properties);
            //CancellationToken token = properties.OnAppDisposing;
            //token.Register(MassTransitMessageQueue.Stop);
        }

        private void ClearBundleTransforms()
        {
#if DEBUG
            foreach (var bundle in BundleTable.Bundles)
            {
                bundle.Transforms.Clear();
            }
#endif
        }
    }
}