using System.Text.RegularExpressions;

namespace Test.API.Authentication.Interfaces
{
    public interface IAuthenticationConfiguration
    {
        IEnumerable<string> ValidIPs { get; }
        IEnumerable<string> ValidAudiences { get; }
        IEnumerable<string> ValidIssuers { get; }
        bool MultiSession { get; }
        bool MultiTenant { get; }
        IDictionary<string, string> URLPermission { get; } // <URL, Permission>
        IDictionary<string, string> URLPolicy { get; }
        IDictionary<string, string> URLTenant { get; }
        // TODO: Mapper of DeviceType => URL {Make decision based on device type Android, iOS, Web, Windows, Linux, MacOs, ...}
        // TODO: Mapper of SessionID => URL // Ban the session
        // TODO: Overall status of authentication server : OK, Out of service
        // TODO: Maximum number of sessions
        // TODO: Maximum number of tokens
        // TODO: Maximum number of devices
    }

    public class AuthenticationConfiguration : IAuthenticationConfiguration
    {
        public IEnumerable<string> ValidIPs { get; set; } = new List<string>();
        public IEnumerable<string> ValidAudiences { get; set; } = new List<string>();
        public IEnumerable<string> ValidIssuers { get; set; } = new List<string>();
        public bool MultiSession { get; set; } = true;
        public bool MultiTenant { get; set; } = true;
        public IDictionary<string, string> URLPermission { get; set; } = new Dictionary<string, string>();
        private IDictionary<Regex, string> URLPermissionRegex => URLPermission.ToDictionary(x => new Regex(x.Key), x => x.Value);
        private Task<bool> HasUrlPermission(string url, IEnumerable<IPermission> permissions) => throw new NotImplementedException();
        public IDictionary<string, string> URLPolicy { get; set; } = new Dictionary<string, string>();
    }
}
