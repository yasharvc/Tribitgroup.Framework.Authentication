using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tribitgroup.Framewok.Identity.Shared.DTO;
using Tribitgroup.Framewok.Identity.Shared.Exceptions;
using Tribitgroup.Framewok.Identity.Shared.Interfaces;
using Tribitgroup.Framewok.Identity.Shared.Models;
using Tribitgroup.Framewok.Shared.Extensions;

namespace Tribitgroup.Framewok.Identity.Server
{
    public class IdentityServerService<TUser, TRole, TPermission> : IIdentityServerService<TPermission>
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
        UserManager<ApplicationUser> UserManager { get; }
        RoleManager<ApplicationRole> RoleManager { get; }

        public IdentityServerService(
            IIdentityDbContext<TUser, TRole, TPermission> dbContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager
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
            var userExists = await IdentityDbContext.GetUserDbSet().FirstOrDefaultAsync(m=>m.UserName == username);
            if (userExists != null)
                throw new UserExistsException();

            ApplicationUser user = new()
            {
                Email = input.Email,
                SecurityStamp = BasicTypesExtensions.GetSequentialGuid().ToString(),
                UserName = input.Username
            };

            var result = await UserManager.CreateAsync(user, password);
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

        private async Task<TUser> GetUserByIdAsync(Guid userId) => (TUser)await UserManager.FindByIdAsync(userId.ToString()) ?? throw new UserNotFoundException();

        public async Task<Guid> AddRoleAsync(string roleName)
        {
            if (!await RoleManager.RoleExistsAsync(roleName))
                await RoleManager.CreateAsync(new ApplicationRole(roleName));

            var role = await RoleManager.FindByNameAsync(roleName);

            return role.Id;
        }

        public Task<IEnumerable<TPermission>> CreatePermissionAsync(params TPermission[] permissions) => PermissionRepo.CreatePermissionAsync(permissions);

        public Task<IEnumerable<TPermission>> GetAllPermissionAsync() => PermissionRepo.GetAllAsync();
    }
}
