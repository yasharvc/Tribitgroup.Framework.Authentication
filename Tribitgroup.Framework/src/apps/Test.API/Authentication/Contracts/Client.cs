using Test.API.Authentication.Enums;

namespace Test.API.Authentication.Contracts
{
    public class Client<Tenant, Policy, Role, Permission> : IClient<Tenant, Policy, Role, Permission>
        where Role : IRole
        where Permission : IPermission
        where Tenant : ITenant
        where Policy : IPolicy
    {
        public IEnumerable<Role> Roles { get; set; } = new List<Role>();
        public IEnumerable<Permission> Permissions { get; set; } = new List<Permission>();
        public IEnumerable<Tenant> Tenants { get; set; } = new List<Tenant>();
        public IEnumerable<Policy> Policies { get; set; } = new List<Policy>();
        public string SessionId { get; set; } = string.Empty;
        public string ApplicationToken { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string ClientToken { get; set; } = string.Empty;
        public string IP { get; set; } = string.Empty;
        public DeviceTypeEnum DeviceType { get; set; } = DeviceTypeEnum.Web;
        public string RequestedUrl { get; set; } = string.Empty;
        public HttpCommandEnum HttpCommand { get; set; } = HttpCommandEnum.GET;
    }
}
