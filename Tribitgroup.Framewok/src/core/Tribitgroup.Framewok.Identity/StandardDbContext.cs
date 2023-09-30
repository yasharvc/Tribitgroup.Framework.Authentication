using Microsoft.EntityFrameworkCore;
using Tribitgroup.Framewok.Identity.Shared.Models;

namespace Tribitgroup.Framewok.Identity
{
    public class StandardDbContext : GenericIdentityDbContext<ApplicationUser, ApplicationRole>
    {
        public StandardDbContext(DbContextOptions<GenericIdentityDbContext<ApplicationUser, ApplicationRole>> options) : base(options)
        {
        }
    }
}