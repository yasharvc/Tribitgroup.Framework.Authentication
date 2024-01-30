using Test.API.Authentication.Interfaces;

namespace Test.API.Authentication
{
    public class Authenticator<Tenant, Policy, Role, Permission>
        where Role : IRole
        where Permission : IPermission
        where Tenant : ITenant
        where Policy : IPolicy
    {
        IEnumerable<IPreauthenticate<Tenant, Policy, Role, Permission>> Preauthenticates { get; } = new List<IPreauthenticate<Tenant, Policy, Role, Permission>>();
        IEnumerable<IAuthenticateStep<Tenant, Policy, Role, Permission>> AuthenticateSteps { get; } = new List<IAuthenticateStep<Tenant, Policy, Role, Permission>>();
    }
}
