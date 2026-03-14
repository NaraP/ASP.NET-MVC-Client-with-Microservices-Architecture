using System.Text;

namespace ECommerce.OrderApi.Middleware
{
    public class RequestResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestResponseLoggingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            // Log Request
            context.Request.EnableBuffering();

            var requestBody = await ReadRequestBody(context.Request);

            _logger.LogInformation(
                "HTTP Request Information: Method={Method} Path={Path} Body={Body}",
                context.Request.Method,
                context.Request.Path,
                requestBody);

            // Capture Response
            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            var responseText = await ReadResponseBody(context.Response);

            _logger.LogInformation(
                "HTTP Response Information: StatusCode={StatusCode} Body={Body}",
                context.Response.StatusCode,
                responseText);

            await responseBody.CopyToAsync(originalBodyStream);
        }

        private static async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.Body.Position = 0;

            using var reader = new StreamReader(
                request.Body,
                Encoding.UTF8,
                leaveOpen: true);

            var body = await reader.ReadToEndAsync();

            request.Body.Position = 0;

            return body;
        }

        private static async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);

            var text = await new StreamReader(response.Body).ReadToEndAsync();

            response.Body.Seek(0, SeekOrigin.Begin);

            return text;
        }
    }
}
