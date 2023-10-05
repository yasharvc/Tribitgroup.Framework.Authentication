using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Models
{
    public class ApplicationPermission : Entity
    {
        public string Name { get; set; } = string.Empty;
        public ApplicationPermission(string name)
        {
            Name = name;
        }
    }
}