using Test.API.Authentication.Enums;

namespace Test.API.Authentication.Contracts
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
        string SessionId { get; }
        string ApplicationToken { get; }//Machine to machine token
        string TenantId { get; }
        string ClientToken { get; }//User login token
        string IP { get; }
        DeviceTypeEnum DeviceType { get; }
        string RequestedUrl { get; }
        HttpCommandEnum HttpCommand { get; }
    }

    public class HttpClient<Tenant, Policy, Role, Permission> : IClient<Tenant, Policy, Role, Permission>
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
        public HttpContext HttpContext { get; set; }
    }
}
