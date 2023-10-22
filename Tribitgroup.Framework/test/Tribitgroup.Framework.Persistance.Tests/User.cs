using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Dapper.Tests
{
    public class User : AggregateRoot
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }
}
