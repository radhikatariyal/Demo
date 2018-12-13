using Patient.Demographics.CrossCutting.Identity;
using System;

using System.Data.Entity.Migrations;
using System.Linq;
using Patient.Demographics.Data.Entities;

namespace Patient.Demographics.Data.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<SqlDataContext>
    {
        public static class AspRoles
        {
            public static Guid AdminRoleId = Guid.Parse("31f116fc-b013-4be5-b40e-a67800ac2ad0");
            public static Guid SupplierRoleId = Guid.Parse("476F1294-EC9B-4488-B450-8EE00E89A62E");
            public static string AdminRoleName = "admin";
            public static string SupplierRoleName = "supplier";

        }

        public static readonly Guid AdminUserId = Guid.Parse("618E6FAB-3542-4360-9CA9-02485B652947");

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(SqlDataContext context)
        {
            AddApplicationRoles(context);
            AddDummyUsers(context);
            context.SaveChanges();
        }
        private void AddApplicationRoles(SqlDataContext context)
        {
            context.Roles.AddOrUpdate(new AspNetRole { Id = AspRoles.AdminRoleId, Name = AspRoles.AdminRoleName });

        }
        private void AddDummyUsers(SqlDataContext context)
        {
            var users = TestUsers.Users.Select(user => new UserEntity
            {
                Id = user.Id,
                Forename = user.ForeName,
                Surname = user.SurName,
                Gender = user.Gender,                
                DateOfBirth = user.DateOfBirth,
                HomeNumber=user.HomeNumber,
                MobileNumber=user.MobileNumber,
                WorkNumber=user.WorkNumber
              
            });

            foreach (var user in users)
            {
                AddUserIfItDoesntExist(context, user);
            }

            //context.UsersEntities.AddOrUpdate(o => o.Id, users.ToArray());
        }

        private void AddUserIfItDoesntExist(SqlDataContext context, UserEntity user)
        {
            if (!context.UsersEntities.Any(t => t.Id == user.Id))
            {
                context.UsersEntities.Add(user);
            }
        }
    }
}
public static class SystemDate
{
    [ThreadStatic]
    private static Func<DateTimeOffset> _nowOffSet;

    public static Func<DateTimeOffset> NowOffset
    {
        get
        {
            if (_nowOffSet == null)
            {
                _nowOffSet = () => DateTimeOffset.Now;
            }
            return _nowOffSet;
        }
        set { _nowOffSet = value; }
    }


    public static void Reset()
    {
        NowOffset = null;
    }
}

