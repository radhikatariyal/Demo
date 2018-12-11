using Patient.Demographics.CrossCutting.Identity;
using Patient.Demographics.Data;
using Patient.Demographics.Data.Entities;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patient.Demographics.Tests.Common.Mocks
{
    public static class QueryModelDataExtensions
    {
       
        public static DbSet<UserEntity> SetupUserQuery(this IQueryModelData queryModelData, params UserEntity[] entities)
        {
            return SetupProperty(() => queryModelData.UsersQuery, entities);
        }
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
