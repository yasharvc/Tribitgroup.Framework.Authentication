using Tribitgroup.Framewok.Shared.Entities;

namespace Tribitgroup.Framewok.Identity.Models
{
    public record Tenant(string PathToParent, string ShortKey, string Title) : EntityRecord { }
}