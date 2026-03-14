namespace ECommerce.MVC.Services;

/// <summary>
/// DelegatingHandler automatically adds "Authorization: Bearer {token}"
/// to every outbound HttpClient request to the backend APIs.
/// Registered once, applied to all three typed clients.
/// </summary>
public class JwtBearerHandler : DelegatingHandler
{
    private const string TokenSessionKey = "JwtToken";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtBearerHandler(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString(TokenSessionKey);

        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var respponse = await base.SendAsync(request, cancellationToken);

        return respponse;
    }
}
