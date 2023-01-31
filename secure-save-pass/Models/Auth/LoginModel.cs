using System.ComponentModel.DataAnnotations;

namespace secure_save_pass.Models.Auth
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
