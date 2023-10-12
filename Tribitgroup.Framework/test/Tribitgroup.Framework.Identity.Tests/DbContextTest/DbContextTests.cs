using Microsoft.Extensions.DependencyInjection;
using Tribitgroup.Framework.Test.Helper;
namespace Tribitgroup.Framework.Identity.Tests.DbContextTest
{
    public class DbContextTests : BaseTestClass<SampleDbContext>
    {
        public DbContextTests() : base(nameof(SampleDbContext))
        {   
        }
    }
}
