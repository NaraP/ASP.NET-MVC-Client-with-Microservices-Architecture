namespace ECommerce.MVC.Models
{
    // Helper to deserialize paged response
    public class PagedResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
