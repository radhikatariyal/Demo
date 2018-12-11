using Patient.Demographics.CrossCutting.Identity;
using Patient.Demographics.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Threading.Tasks;
using RefactorThis.GraphDiff;


namespace Patient.Demographics.Data
{
    public interface ISqlDataContext : IDisposable
    {
        IDbSet<T> GetIDbSet<T>() where T : class;
        void SetModified(object entity);
        void EnableAutoDetectChanges();
        void DisableAutoDetectChanges();
        void DetectChanges();
        IDbSet<AspNetUser> Users { get; set; }
        string ConnectionString { get; }
        DbSet<AspNetUserRole> IdentityUserRoles { get; set; }
       
        DbSet<UserEntity> UsersEntities { get; set; }
      
        Task<int> SaveChangesAsync();
      
        Task DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        IUpdateAsyncResult<T> UpdateAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        void BulkInsert<T>(IEnumerable<T> entities, int? batchSize = null);
        Task ExecuteStoredProcedureAsync(StoredProcedures storedProcedure, params object[] parameter);
        Task<List<T>> ExecuteStoredProcedureResultListAsync<T>(StoredProcedures storedProcedure, params object[] parameter);
        Task<T> ExecuteStoredProcedureResultSingleAsync<T>(StoredProcedures storedProcedure, params object[] parameter);
        Task<List<T>> SqlQuery<T>(string sql, params SqlParameter[] parameters);
        T UpdateGraph<T>(T entity, Expression<Func<IUpdateConfiguration<T>, object>> mapping,
            Action<object, object> trackEntityUpdate = null) where T : class, new();
      
    }
}