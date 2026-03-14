using System.ComponentModel.DataAnnotations;

namespace ECommerce.MVC.Models
{
    /// <summary>Role returned by GET /api/roles.</summary>
    public class RoleApiDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int? UserCount { get; set; }   // nullable — not all endpoints return this
    }
}
