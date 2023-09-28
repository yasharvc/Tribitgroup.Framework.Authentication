namespace Tribitgroup.Framewok.Identity.Shared.Models
{
    public class UserInfo
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IEnumerable<Role> Roles { get; set; } = Enumerable.Empty<Role>();
        public IEnumerable<Tenant> Tenants { get; set; } = Enumerable.Empty<Tenant>();
        public IEnumerable<Permission> Permissions { get; set; } = Enumerable.Empty<Permission>();

    }
}