using NSubstitute;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patient.Demographics.Tests.Common.Mocks
{
    public static class MockDbSetFactory
    {
        public static DbSet<TEntity> CreateMockAsyncableDbSet<TEntity>(IQueryable<TEntity> queryableData) where TEntity : class
        {
            var mockSet = Substitute.For<DbSet<TEntity>, IDbAsyncEnumerable<TEntity>, IQueryable<TEntity>>();

            ((IDbAsyncEnumerable<TEntity>)mockSet).GetAsyncEnumerator()
                                                  .Returns(new MockDbAsyncEnumerator<TEntity>(queryableData.GetEnumerator()));

            ((IQueryable<TEntity>)mockSet).Provider
                                          .Returns(new MockDbAsyncQueryProvider<TEntity>(queryableData.Provider));

            ((IQueryable<TEntity>)mockSet).Provider
                                          .Returns(new MockDbAsyncQueryProvider<TEntity>(queryableData.Provider));

            ((IQueryable<TEntity>)mockSet).Expression
                                          .Returns(queryableData.Expression);
            ((IQueryable<TEntity>)mockSet).ElementType
                                          .Returns(queryableData.ElementType);
            ((IQueryable<TEntity>)mockSet).GetEnumerator()
                                          .Returns(queryableData.GetEnumerator());


            return mockSet;
        }
    }
}
