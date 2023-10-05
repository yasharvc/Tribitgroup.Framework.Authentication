using System.ComponentModel.DataAnnotations;
using Tribitgroup.Framewok.Shared.DTO;

namespace Tribitgroup.Framewok.Identity.Shared.DTO
{
    public class LoginWithUsernamePasswordInputDTO : InputDTO
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}