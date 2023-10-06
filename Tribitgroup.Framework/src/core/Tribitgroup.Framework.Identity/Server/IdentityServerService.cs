using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tribitgroup.Framework.Identity.Shared.DTO;
using Tribitgroup.Framework.Identity.Shared.Exceptions;
using Tribitgroup.Framework.Identity.Interfaces;
using Tribitgroup.Framework.Identity.Models;
using Tribitgroup.Framework.Shared.Extensions;

namespace Tribitgroup.Framework.Identity.Server
{
    public class IdentityServerService<TUser, TRole, TPermission> : IIdentityServerService<TUser, TRole ,TPermission>
        where TUser : ApplicationUser
        where TRole : ApplicationRole
        where TPermission : ApplicationPermission
    {

        class PermissionRepository
        {
            IIdentityDbContext<TUser, TRole, TPermission> IdentityDbContext { get; }
            public PermissionRepository(IIdentityDbContext<TUser, TRole, TPermission> identityDbContext)
            {
                IdentityDbContext = identityDbContext;
            }
            public async Task<IEnumerable<TPermission>> CreatePermissionAsync(params TPermission[] permissions)
            {
                await IdentityDbContext.Permissions.AddRangeAsync(permissions);
                await (IdentityDbContext as DbContext ?? throw new InvalidCastException()).SaveChangesAsync();
                return permissions;
            }

            public async Task<IEnumerable<TPermission>> GetAllAsync() => (await IdentityDbContext.Permissions.ToListAsync()).AsEnumerable();
        }

        PermissionRepository PermissionRepo { get; init; }

        IIdentityDbContext<TUser, TRole, TPermission> IdentityDbContext { get; }
        UserManager<TUser> UserManager { get; }
        RoleManager<TRole> RoleManager { get; }

        public IdentityServerService(
            IIdentityDbContext<TUser, TRole, TPermission> dbContext,
            UserManager<TUser> userManager,
            RoleManager<TRole> roleManager
            )
        {
            IdentityDbContext = dbContext;
            UserManager = userManager;
            RoleManager = roleManager;
            PermissionRepo = new PermissionRepository(dbContext);
        }


        public async Task<Guid> RegisterAsync(RegisterWithUsernameEmailPasswordInputDTO input, CancellationToken cancellationToken = default)
        {
            var username = input.Username ?? "";
            var password = input.Password ?? "";
            var userExists = await IdentityDbContext.GetUserDbSet().FirstOrDefaultAsync(m =>m.UserName == username, cancellationToken: cancellationToken);
            if (userExists != null)
                throw new UserExistsException();

            var user = new ApplicationUser()
            {
                Email = input.Email,
                SecurityStamp = BasicTypesExtensions.GetSequentialGuid().ToString(),
                UserName = input.Username
            };

            var result = await UserManager.CreateAsync((TUser)user, password);
            if (!result.Succeeded)
                throw new Exception();

            return user.Id;
        }

        public async Task AddRoleToUserAsync(Guid userId, params string[] roleNames)
        {
            var user = await GetUserByIdAsync(userId);

            foreach (var roleName in roleNames)
            {
                if (await RoleManager.RoleExistsAsync(roleName))
                {
                    await UserManager.AddToRoleAsync(user, roleName);
                }
            }
        }

        private async Task<TUser> GetUserByIdAsync(Guid userId) => await UserManager.FindByIdAsync(userId.ToString()) ?? throw new UserNotFoundException();

        public async Task<TRole> CreateRoleAsync(string roleName)
        {
            if (!await RoleManager.RoleExistsAsync(roleName))
                await RoleManager.CreateAsync((TRole)new ApplicationRole(roleName));

            var role = await RoleManager.FindByNameAsync(roleName);

            return role ?? throw new Exception();
        }

        public Task<IEnumerable<TPermission>> CreatePermissionAsync(params TPermission[] permissions) => PermissionRepo.CreatePermissionAsync(permissions);

        public Task<IEnumerable<TPermission>> GetAllPermissionAsync() => PermissionRepo.GetAllAsync();

        public async Task<IEnumerable<TRole>> GetAllRolesAsync() => (await RoleManager.Roles.ToListAsync()).AsEnumerable();
    }
}
