namespace Test.API.Authentication.Interfaces
{
    public interface IClient<Tenant, Policy, Role, Permission> 
        where Role : IRole
        where Permission : IPermission
        where Tenant : ITenant
        where Policy : IPolicy
    {
        IEnumerable<Role> Roles { get; }
        IEnumerable<Permission> Permissions { get; }
        IEnumerable<Tenant> Tenants{ get; }
        IEnumerable<Policy> Policies { get; }
        
    }
}
