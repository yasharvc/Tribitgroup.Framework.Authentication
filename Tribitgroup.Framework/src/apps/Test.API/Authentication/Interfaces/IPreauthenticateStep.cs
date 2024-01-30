namespace Test.API.Authentication.Interfaces
{
    internal interface IPreauthenticateStep<Tenant, Policy, Role, Permission>
        where Tenant : ITenant
        where Policy : IPolicy
        where Role : IRole
        where Permission : IPermission
    {
        Task<IClient<Tenant, Policy, Role, Permission>> ExecuteAsync(IClient<Tenant, Policy, Role, Permission> client, IAuthenticationConfiguration configuration);
    }
}