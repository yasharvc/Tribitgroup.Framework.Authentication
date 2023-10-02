using System.ComponentModel.DataAnnotations;
using Tribitgroup.Framewok.Shared.DTO;

namespace Tribitgroup.Framewok.Identity.Shared.DTO
{
    public class CreatePermissionInputDTO : InputDTO
    {
        [Required]
        public string PermissionName { get; set; } = string.Empty;
    }
}