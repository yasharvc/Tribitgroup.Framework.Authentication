using Test.API.Authentication.Contracts;
using Test.API.Authentication.Enums;

namespace Test.API.Test
{
    public class DeviceTypeSetter : IPreauthenticatePipeStep<Tenant, Policy, Role, Permission>
    {
        public Task<IClient<Tenant, Policy, Role, Permission>> ExecuteAsync(IClient<Tenant, Policy, Role, Permission> client, IAuthenticationConfiguration configuration)
        {
            var httpClient = (client as HttpClient<Tenant, Policy, Role, Permission> ?? throw new Exception());

            var deviceTypeHeader = httpClient.HttpContext.Request.Headers["DeviceType"].FirstOrDefault() ?? "";

            httpClient.DeviceType = deviceTypeHeader.ToLower() switch
            {
                "windows" => DeviceTypeEnum.Windows,
                "macos" => DeviceTypeEnum.MacOS,
                "linux" => DeviceTypeEnum.Linux,
                "application" => DeviceTypeEnum.Application,
                "android" => DeviceTypeEnum.Android,
                _ => DeviceTypeEnum.Web
            };
            
            return Task.FromResult((IClient<Tenant, Policy, Role, Permission>)httpClient);
        }
    }
}
