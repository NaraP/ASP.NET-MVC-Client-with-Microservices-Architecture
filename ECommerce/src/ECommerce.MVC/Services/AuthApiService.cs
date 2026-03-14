using ECommerce.MVC.IServices;
using ECommerce.MVC.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace ECommerce.MVC.Services
{
    /// <summary>
    /// Typed <see cref="HttpClient"/> implementation of <see cref="IAuthApiService"/>.
    /// The HTTP client is pre-configured in Program.cs with the AuthApi base URL.
    /// The <see cref="JwtBearerHandler"/> message handler automatically injects the
    /// Bearer token from the session for calls that require authentication.
    /// </summary>
    public class AuthApiService : IAuthApiService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json;

        public AuthApiService(HttpClient http)
        {
            _http = http;
            _json = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // ── POST /api/auth/login ──────────────────────────────────────────────────

        public async Task<AuthApiResponse?> LoginAsync(string email, string password)
        {
            var payload = new AuthLoginRequest { Email = email, Password = password };
            var response = await PostJsonAsync("/api/auth/login", payload);

            if (response is null || !response.IsSuccessStatusCode)
                return null;

            return await DeserializeAsync<AuthApiResponse>(response);
        }

        // ── POST /api/auth/register ───────────────────────────────────────────────

        public async Task<(bool Success, string? Error, AuthApiResponse? Response)> RegisterAsync(
            RegisterViewModel model)
        {
            var payload = new AuthRegisterRequest
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                RoleName = model.RoleName,
            };

            var response = await PostJsonAsync("/api/auth/register", payload);

            if (response is null)
                return (false, "Could not reach the authentication service. Please try again.", null);

            if (!response.IsSuccessStatusCode)
            {
                // AuthApi returns a ProblemDetails body with a "detail" field on errors
                var error = await ExtractErrorAsync(response);
                return (false, error, null);
            }

            var result = await DeserializeAsync<AuthApiResponse>(response);
            return result is not null
                ? (true, null, result)
                : (false, "Registration succeeded but the response could not be parsed.", null);
        }

        // ── GET /api/auth/check-email ─────────────────────────────────────────────

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            try
            {
                var response = await _http.GetAsync(
                    $"/api/auth/check-email?email={Uri.EscapeDataString(email)}");

                if (!response.IsSuccessStatusCode) return true; // assume available on failure

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<EmailCheckResult>(json, _json);
                return result?.Available ?? true;
            }
            catch
            {
                return true; // graceful degradation — don't block registration on network error
            }
        }
        public async Task<bool> ResetPasswordAsync(ResetPasswordViewModel request)
        {
            var response = await _http.PostAsJsonAsync(
                "api/auth/reset-password",
                request);

            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<bool>();

            return result;
        }

        // ── POST /api/auth/logout ─────────────────────────────────────────────────

        public async Task LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken)) return;

            try
            {
                var payload = new AuthRefreshRequest { RefreshToken = refreshToken };
                await PostJsonAsync("/api/auth/logout", payload);
                // Fire-and-forget — session is cleared locally regardless of the API result
            }
            catch
            {
                // Swallow — local session will be cleared by the controller
            }
        }

        // ── POST /api/auth/refresh ────────────────────────────────────────────────

        public async Task<AuthApiResponse?> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken)) return null;

            var payload = new AuthRefreshRequest { RefreshToken = refreshToken };
            var response = await PostJsonAsync("/api/auth/refresh", payload);

            if (response is null || !response.IsSuccessStatusCode) return null;

            return await DeserializeAsync<AuthApiResponse>(response);
        }

        // ── GET /api/auth/me ──────────────────────────────────────────────────────

        public async Task<AuthUserDto?> GetProfileAsync()
        {
            try
            {
                var response = await _http.GetAsync("/api/auth/me");
                if (!response.IsSuccessStatusCode) return null;
                return await DeserializeAsync<AuthUserDto>(response);
            }
            catch
            {
                return null;
            }
        }

        // ── Private helpers ───────────────────────────────────────────────────────

        private async Task<HttpResponseMessage?> PostJsonAsync<T>(string url, T payload)
        {
            try
            {
                var body = new StringContent(
                    JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                return await _http.PostAsync(url, body);
            }
            catch
            {
                return null;
            }
        }

        private async Task<T?> DeserializeAsync<T>(HttpResponseMessage response)
        {
            try
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json, _json);
            }
            catch
            {
                return default;
            }
        }

        private async Task<string> ExtractErrorAsync(HttpResponseMessage response)
        {
            try
            {
                var json = await response.Content.ReadAsStringAsync();
                var problem = JsonSerializer.Deserialize<ProblemResult>(json, _json);
                return problem?.Detail ?? problem?.Title ?? "An unexpected error occurred.";
            }
            catch
            {
                return "An unexpected error occurred.";
            }
        }

        // ── Private DTOs for parsing API error/check bodies ───────────────────────
        private sealed class EmailCheckResult { public bool Available { get; set; } }

        private sealed class ProblemResult
        {
            public string? Title { get; set; }
            public string? Detail { get; set; }
        }
    }
}
