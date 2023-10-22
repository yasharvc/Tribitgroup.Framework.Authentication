using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tribitgroup.Framework.Identity.EF.Repositories;
using Tribitgroup.Framework.Persistance.Tests.DbContext;
using Tribitgroup.Framework.Test.Helper;

namespace Tribitgroup.Framework.Dapper.Tests
{
    public class EfTests : BaseTestClass<SampleDbContext>
    {
        GenericRepository<User,SampleDbContext,Guid> repository;
        public EfTests() : base(nameof(EfTests))
        {
            repository = GetService<GenericRepository<User,SampleDbContext,Guid>>();
        }

        protected override void AddServices(ServiceCollection services)
        {
            base.AddServices(services);
            services.AddSingleton<GenericRepository<User, SampleDbContext, Guid>>();
        }

        [Fact]
        public async Task InsertOne_Should_Add_User_Into_DbContext()
        {
            var user = new User
            {
                DateOfBirth = DateTime.Now,
                Password = "password",
                Username = "username",
            };

            await repository.InsertOneAsync(user);

            GetDbContext().Users.Count().ShouldBe(1);
            GetDbContext().Users.First().Password.ShouldBe(user.Password);
            GetDbContext().Users.First().Id.ShouldBe(user.Id);
            GetDbContext().Users.First().DateOfBirth.ShouldBe(user.DateOfBirth);
        }
    }
}
