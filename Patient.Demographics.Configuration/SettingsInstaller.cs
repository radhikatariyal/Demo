using Patient.Demographics.Common.Configuration;
using Patient.Demographics.Common.Settings;
using Patient.Demographics.Configuration.Settings;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Patient.Demographics.Configuration
{
    public class SettingsInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IAuthenticationServerSettings>().ImplementedBy<AuthenticationServerConfigSettings>());
            container.Register(
                Component.For<IDataSettings>().ImplementedBy<DataConfigSettings>());
            container.Register(
                Component.For<IMessageQueueSettings>().ImplementedBy<MessageQueueConfigSettings>());
        }
    }
}