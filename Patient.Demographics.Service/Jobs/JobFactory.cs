using System.Linq;
using Castle.Windsor;
using Quartz;
using Quartz.Spi;

namespace Patient.Demographics.Service.Jobs
{
    public class JobFactory : IJobFactory
    {
        private readonly IWindsorContainer _container;

        public JobFactory(IWindsorContainer container)
        {
            _container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var all = _container.ResolveAll<IJob>();
            return all.First(j => j.GetType() == bundle.JobDetail.JobType);
        }

        public void ReturnJob(IJob job)
        {
        }
    }
}
