using Patient.Demographics.Data;
using Castle.MicroKernel;

namespace Patient.Demographics.Configuration
{
    public class SqlDataContextFactory : ISqlDataContextFactory
    {
        private readonly IKernel _kernel;

        public SqlDataContextFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public ISqlDataContext Create()
        {
            return _kernel.Resolve<ISqlDataContext>();
        }

        public void Release(ISqlDataContext sqlDataContext)
        {
            if (sqlDataContext != null)
            {
                _kernel.ReleaseComponent(sqlDataContext);
            }
        }
    }
}