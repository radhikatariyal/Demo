using Patient.Demographics.Service.FileUploads;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Patient.Demographics.Service.WindsorInstallers
{
    public class ValidationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IWindsorContainer>().Instance(container));

            container.Register(Classes.FromThisAssembly()
                    .BasedOn<IUploadValidator>()
                    .WithServiceAllInterfaces()
                    .LifestyleTransient());

            container.Register(Component.For<IUploadValidatorFactory>()
                    .ImplementedBy<UploadValidatorFactory>()
                    .LifestyleTransient());
        }
    }
}