using System.ComponentModel.DataAnnotations;

namespace ECommerce.AuthApi.Models
{
    public class ResetPasswordViewModel
    {
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
