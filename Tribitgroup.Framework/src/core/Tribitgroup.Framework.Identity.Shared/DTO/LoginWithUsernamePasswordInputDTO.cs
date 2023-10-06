using System.ComponentModel.DataAnnotations;
using Tribitgroup.Framework.Shared.DTO;

namespace Tribitgroup.Framework.Identity.Shared.DTO
{
    public class LoginWithUsernamePasswordInputDTO : InputDTO
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}