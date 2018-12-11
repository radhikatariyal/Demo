using Patient.Demographics.Events;
using Patient.Demographics.Events.BatchProces;
using Patient.Demographics.Service.EventHandlers;
using Patient.Demographics.Service.FileUploads;
using Patient.Demographics.Service.FileUploads.Importers.User;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System.IO.Abstractions;

namespace Patient.Demographics.Service.WindsorInstallers
{
    public class ImportersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly()
                                      .BasedOn<IUploadImporter>()
                                      .WithServiceAllInterfaces()
                                      .LifestyleTransient());

            container.Register(Component.For<IUploadImporterFactory>()
                                        .ImplementedBy<UploadImporterFactory>()
                                        .LifestyleTransient());

            container.Register(Component.For<IRowToImportMapper>()
                                        .ImplementedBy<RowToImportMapper>()
                                        .LifestyleTransient());

            container.Register(Component.For<IEventHandler<BatchProcessNotificationEvent>>()
                                        .ImplementedBy<UploadNotificationEventHandler>()
                                        .LifestyleTransient());

            container.Register(Component.For<ICategoryImportPersistance>()
                            .ImplementedBy<CategoryImportPersistance>()
                            .LifestyleTransient());

            //container.Register(Component.For<ICategoryImporter>()
            //    .ImplementedBy<Cata>()
            //    .LifestyleTransient());

            container.Register(Component.For<IFileSystem>().ImplementedBy<FileSystem>().LifestyleTransient());
        }
    }
}