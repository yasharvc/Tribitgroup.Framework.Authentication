using System.ComponentModel.DataAnnotations.Schema;
using Tribitgroup.Framework.Shared.Entities;

namespace Tribitgroup.Framework.Identity.Models
{
    public class UserRefreshToken : Entity
    {
        [ForeignKey("UserId")]
        public Guid ApplicationUserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ValidUntil { get; set; }
        public int UsedCount { get; set; } = 0;
    }
}