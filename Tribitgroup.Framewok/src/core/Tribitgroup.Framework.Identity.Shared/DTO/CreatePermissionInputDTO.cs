using System.ComponentModel.DataAnnotations;
using Tribitgroup.Framework.Shared.DTO;

namespace Tribitgroup.Framework.Identity.Shared.DTO
{
    public class CreatePermissionInputDTO : InputDTO
    {
        [Required]
        public string PermissionName { get; set; } = string.Empty;
    }
}