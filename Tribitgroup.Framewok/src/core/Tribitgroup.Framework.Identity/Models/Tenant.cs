using Tribitgroup.Framework.Shared.Entities;

namespace Tribitgroup.Framework.Identity.Models
{
    public record Tenant(string PathToParent, string ShortKey, string Title) : EntityRecord { }
}