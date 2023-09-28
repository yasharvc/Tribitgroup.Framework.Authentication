namespace Tribitgroup.Framewok.Identity.Shared.Models
{
    public record Tenant(Guid Id, string PathToParent, string ShortKey, string Title)
    {
    }
}