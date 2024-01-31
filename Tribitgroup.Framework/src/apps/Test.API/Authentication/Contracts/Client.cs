using Test.API.Authentication.Enums;
using Test.API.Authentication.Models;

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
        public string IPv4 { get; set; } = string.Empty;
        public string IPv6 {get;set;} = string.Empty;
        public int Port { get; set; } = 80;
        public DeviceTypeEnum DeviceType { get; set; } = DeviceTypeEnum.Web;
        public string RequestedUrl { get; set; } = string.Empty;
        public string QueryString { get; set; } = string.Empty;
        public HttpCommandEnum HttpCommand { get; set; } = HttpCommandEnum.GET;
        public string HttpScheme { get; set; } = "http";
        public string HttpProtocol { get; set; } = "HTTP/1.1";

        public string Subdomain { get; set; } = string.Empty;

        public string Domain { get; set; } = string.Empty;

        public LocalizationInfo Localization { get; set; } = new LocalizationInfo();
    }
}
