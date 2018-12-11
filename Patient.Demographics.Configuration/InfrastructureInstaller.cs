
using Patient.Demographics.Domain;
using Patient.Demographics.Infrastructure;
using Patient.Demographics.Infrastructure.Logging;
using Patient.Demographics.Infrastructure.Mappers;
using Patient.Demographics.Infrastructure.Storage.Aggregates;
using Patient.Demographics.Infrastructure.Storage.BulkImports;
using Patient.Demographics.Data;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Patient.Demographics.Configuration
{
    public class InfrastructureInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ISqlDataContext>()
                                        .ImplementedBy<SqlDataContext>()
                                        .LifestyleTransient()
                                        .Named("DataContext"));

            container.Register(Component.For<ISqlDataContextFactory>()
                                        .ImplementedBy<SqlDataContextFactory>()
                                        .LifestyleTransient());

            container.Register(Component.For<IDatabaseInitialise>().ImplementedBy<DatabaseInitialise>().LifestyleTransient());

            container.Register(Component.For(typeof(IRepository<>)).ImplementedBy(typeof(Repository<>)).LifestyleTransient());

            container.Register(Component.For<IAggregateStorageFactory>().AsFactory().LifestyleTransient());

            container.Register(Classes.FromAssemblyContaining<UserStorage>()
                                        .BasedOn(typeof(IAggregateStorage<>))
                                        .WithServiceFirstInterface()
                                        .LifestyleTransient());

            container.Register(Component.For<IQueryModelData>()
                                        .ImplementedBy<QueryModelData>()
                                        .LifestyleTransient());

           
            

            container.Register(Component.For<IUserActivityLogger>()
                                        .ImplementedBy<UserActivityLogger>()
                                        .LifestyleTransient());

            container.Register(Component.For<IBulkInserterFactory>().AsFactory().LifestyleTransient());

            container.Register(Component.For<IBulkInserter>()
                                        .ImplementedBy<BulkInserter>()
                                        .LifestyleTransient());

        }
    }
}