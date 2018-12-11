using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Patient.Demographics.Configuration.Extensions;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Patient.Demographics.Web;

namespace Patient.Demographics.API.Installers
{
    public class ControllersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            const string assemblyNamePrefix = "Patient.Demographics";
            
            // Register all Web API controllers from this assembly
            container.Register(
                UseAssembly.InApplicationDirectory(assemblyNamePrefix)
                    .BasedOn<IHttpController>()
                    .LifestyleScoped());

            // Resolve REST Api
            GlobalConfiguration.Configuration.DependencyResolver = new WindsorHttpDependencyResolver(container);
        }
    }
}