using Patient.Demographics.Common;
using Patient.Demographics.CrossCutting.Identity;
using Patient.Demographics.Data.Entities;
using EntityFramework.BulkInsert.Extensions;
using EntityFramework.Extensions;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RefactorThis.GraphDiff;


namespace Patient.Demographics.Data
{
    public class StoredProcedures : Enumeration
    {
        public static readonly StoredProcedures ValidateStaticAudienceUpload = new StoredProcedures("ValidateStaticAudienceUpload", "exec dbo.ValidateStaticAudienceUpload @uploadId, @audienceId");
        public static readonly StoredProcedures SalesClaimsSearch = new StoredProcedures("SalesClaims_Search", "dbo.SalesClaims_Search @organisationIds, @productMasterId, @salesMasterId, @searchTerm, @salesStartDate, @salesEndDate, @saleClaimStatus, @sortField, @itemsPerPage, @pageNumber, @sortAscending");
        public static readonly StoredProcedures GetProgramCatalogueJson = new StoredProcedures("GetProgramCatalogueJson", "exec dbo.GetProgramCatalogueJson @ProgramCatalogueId");

        public StoredProcedures()
        {
        }

        private StoredProcedures(string name, string displayName) : base(name, displayName)
        {
        }
    }

    public class SqlDataContext : IdentityDbContext<AspNetUser, AspNetRole, Guid, AspNetUserLogin, AspNetUserRole, AspNetUserClaim>, ISqlDataContext
    {
        public SqlDataContext() : base("DefaultConnection")
        {
            Configuration.AutoDetectChangesEnabled = true;
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<UserEntity> UsersEntities { get; set; }
        public virtual DbSet<AspNetUserRole> IdentityUserRoles { get; set; }
     
        public string ConnectionString => Database.Connection.ConnectionString;
      
        public async Task DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            await GetIDbSet<T>().Where(predicate).DeleteAsync();
        }

        public IUpdateAsyncResult<T> UpdateAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var updateAsyncResult = new UpdateAsyncResult<T>(GetIDbSet<T>().Where(predicate));
            return updateAsyncResult;
        }

        public void BulkInsert<T>(IEnumerable<T> entities, int? batchSize = null)
        {
            ((DbContext)this).BulkInsert(entities, batchSize);
        }

        public Task ExecuteStoredProcedureAsync(StoredProcedures storedProcedure, params object[] parameter)
        {
            throw new NotImplementedException();
        }

        public async Task ExecuteStoredProcedureResultListAsync(StoredProcedures storedProcedure, params object[] parameters)
        {
            await Database.ExecuteSqlCommandAsync(storedProcedure.DisplayName, parameters);
        }

        public async Task<List<T>> ExecuteStoredProcedureResultListAsync<T>(StoredProcedures storedProcedure, params object[] parameters)
        {
            var raw = Database.SqlQuery<T>(storedProcedure.DisplayName, parameters);
            return await raw.ToListAsync();
        }

        public async Task<T> ExecuteStoredProcedureResultSingleAsync<T>(StoredProcedures storedProcedure, params object[] parameter)
        {
            var raw = Database.SqlQuery<T>(storedProcedure.DisplayName, parameter);
            return await raw.SingleAsync();
        }

        public void ExecuteStoredProcedure(StoredProcedures storedProcedure, params object[] parameters)
        {
            Database.ExecuteSqlCommand(storedProcedure.DisplayName, parameters);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Ignore<IdentityRole>();
            modelBuilder.Ignore<IdentityUserRole>();
            modelBuilder.Ignore<IdentityUserLogin>();
            modelBuilder.Configurations.AddFromAssembly(Assembly.GetAssembly(GetType()));
        }

        public IDbSet<T> GetIDbSet<T>() where T : class
        {
            return Set<T>();
        }

        public void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }

        public void DetectChanges()
        {
            ChangeTracker.DetectChanges();
        }

        public void EnableAutoDetectChanges()
        {
            Configuration.AutoDetectChangesEnabled = true;
        }

        public void DisableAutoDetectChanges()
        {
            Configuration.AutoDetectChangesEnabled = false;
        }

        public async Task<List<T>> SqlQuery<T>(string sql, params SqlParameter[] parameters)
        {
            return await Database.SqlQuery<T>(sql, parameters).ToListAsync();
        }

        public override Task<int> SaveChangesAsync()
        {
            try
            {
                return base.SaveChangesAsync();
            }
            catch (DbEntityValidationException entityValidationException)
            {
                throw GetDecoratedDbEntityValidationException(entityValidationException);
            }
        }

        private DbEntityValidationException GetDecoratedDbEntityValidationException(DbEntityValidationException entityValidationException)
        {
            var validationErrorsBuilder = new StringBuilder();

            foreach (var validationError in entityValidationException.EntityValidationErrors)
            {
                validationErrorsBuilder.AppendFormat("{0} failed validation\n", validationError.Entry.Entity.GetType());
                foreach (var innerValidationError in validationError.ValidationErrors)
                {
                    validationErrorsBuilder.AppendFormat("- {0} : {1}", innerValidationError.PropertyName, innerValidationError.ErrorMessage);
                    validationErrorsBuilder.AppendLine();
                }
            }

            var newErrorMessage = string.Concat("Entity validation failed - errors follow:\n", validationErrorsBuilder);
            return new DbEntityValidationException(newErrorMessage, entityValidationException);
        }

        public T UpdateGraph<T>(T entity, Expression<Func<IUpdateConfiguration<T>, object>> mapping,
            Action<object, object> trackEntityUpdate = null) where T : class, new()
        {
            return ((DbContext)this).UpdateGraph(entity, mapping);
        }
    }

    public interface IUpdateAsyncResult<T> where T : class
    {
        Task Set(Expression<Func<T, T>> expression);
    }

    public class UpdateAsyncResult<T> : IUpdateAsyncResult<T> where T : class
    {
        private readonly IQueryable<T> _query;

        public UpdateAsyncResult(IQueryable<T> query)
        {
            _query = query;
        }

        public async Task Set(Expression<Func<T, T>> expression)
        {
            await _query.UpdateAsync(expression);
        }
    }
}