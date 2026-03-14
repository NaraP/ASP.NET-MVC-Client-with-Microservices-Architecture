using System.ComponentModel.DataAnnotations;

namespace ECommerce.MVC.Models
{
    public class UpdateUserApiRequest
    {
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public bool? IsActive { get; set; }
    }
}
