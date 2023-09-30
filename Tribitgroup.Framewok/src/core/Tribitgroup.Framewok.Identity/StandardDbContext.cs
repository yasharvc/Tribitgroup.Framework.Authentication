using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tribitgroup.Framewok.Identity.Shared.Models;

namespace Tribitgroup.Framewok.Identity
{
    public class StandardDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public StandardDbContext(DbContextOptions<StandardDbContext> options) : base(options)
        {
        }
    }
}