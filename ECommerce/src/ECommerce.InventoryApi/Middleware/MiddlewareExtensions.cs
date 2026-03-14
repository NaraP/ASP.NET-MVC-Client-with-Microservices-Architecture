namespace ECommerce.InventoryApi.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(
            this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}
