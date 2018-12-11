using System.Reflection;
using Patient.Demographics.Configuration;
using Castle.Windsor;
using Patient.Demographics.API.Installers;

namespace Patient.Demographics.API
{
    public class WebApplicationContainer : WindsorContainer
    {
        public WebApplicationContainer(Assembly[] assemblies = null)
        {
            Install(new ApplicationInstaller());
            var webInstaller = new WebInstaller();
            webInstaller.SetAssembly(assemblies);
            Install(webInstaller);
        }
    }
}