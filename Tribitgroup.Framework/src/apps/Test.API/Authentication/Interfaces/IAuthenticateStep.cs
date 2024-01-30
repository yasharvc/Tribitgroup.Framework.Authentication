namespace Test.API.Authentication.Interfaces
{
    internal interface IAuthenticateStep<Tenant, Policy, Role, Permission>
        where Tenant : ITenant
        where Policy : IPolicy
        where Role : IRole
        where Permission : IPermission
    {
    }
}