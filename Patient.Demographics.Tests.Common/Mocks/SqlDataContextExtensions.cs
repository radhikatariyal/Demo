using NSubstitute;
using System;
using System.Linq;
using System.Data.Entity;
using Patient.Demographics.Data;
namespace Patient.Demographics.Tests.Common.Mocks
{
    public static class SqlDataContextExtensions
    {
        


        private static DbSet<T> SetupProperty<T>(Func<IQueryable<T>> property, params T[] entities) where T : class
        {
            var mockDbSet = MockDbSetFactory.CreateMockAsyncableDbSet(entities.AsQueryable());
            property().Returns(mockDbSet);

            mockDbSet.Include(Arg.Any<string>())
                     .Returns(mockDbSet);

            return mockDbSet;
        }
    }
}
