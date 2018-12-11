using Patient.Demographics.Service.FileUploads;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Patient.Demographics.Service.WindsorInstallers
{
    public class QueueInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {

            container.Register(
                Classes.FromThisAssembly()
                    .BasedOn<IUploadStatusChangeQueue>()
                    .WithServiceAllInterfaces()
                    .LifestyleTransient());
        }
    }
}