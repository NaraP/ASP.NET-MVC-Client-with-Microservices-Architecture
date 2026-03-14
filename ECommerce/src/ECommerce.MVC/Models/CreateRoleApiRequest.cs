using System.ComponentModel.DataAnnotations;

namespace ECommerce.MVC.Models
{
    public class CreateRoleApiRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;
    }
}
