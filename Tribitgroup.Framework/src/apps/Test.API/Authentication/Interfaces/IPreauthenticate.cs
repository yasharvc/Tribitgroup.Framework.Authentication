namespace Test.API.Authentication.Interfaces
{
    public interface IPreauthenticate<Tenant, Policy, Role, Permission>
        where Role : IRole
        where Permission : IPermission
        where Tenant : ITenant
        where Policy : IPolicy
    {
        Task<Authenticator<Tenant, Policy, Role, Permission>> ProcessAsync(IClient<Tenant, Policy, Role, Permission> client);
    }
}