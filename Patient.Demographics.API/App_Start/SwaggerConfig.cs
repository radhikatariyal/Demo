using System.Configuration;
using System.Web.Http;
using WebActivatorEx;
using Patient.Demographics.API;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace Patient.Demographics.API
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;
            bool swaggerEnabled;
            bool.TryParse(ConfigurationManager.AppSettings["dev:SwaggerEnabled"], out swaggerEnabled);
            if (swaggerEnabled)
            {
                //GlobalConfiguration.Configuration
                //    .EnableSwagger(c => { c.SingleApiVersion("v1.0", "Patient.Demographics.API"); })
                //    .EnableSwaggerUi();
            }
        }
    }
}
