using ECommerce.MVC.IServices;
using ECommerce.MVC.Models;
using System.Text.Json;

namespace ECommerce.MVC.Services
{
    /// <summary>
    /// Typed HttpClient that calls the Admin endpoints on ECommerce.AuthApi.
    /// Shares the same HttpClient instance as AuthApiService (same base URL and
    /// JwtBearerHandler), registered separately in Program.cs so each has its
    /// own typed client.
    /// </summary>
    public class AdminApiService : IAdminApiService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json;

        public AdminApiService(HttpClient http)
        {
            _http = http;
            _json = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // ── Users ─────────────────────────────────────────────────────────────────

        public async Task<List<AuthUserDto>> GetUsersAsync(bool? isActive = null)
        {
            var url = "/api/users";
            if (isActive.HasValue) url += $"?isActive={isActive.Value.ToString().ToLower()}";
            return await GetListAsync<AuthUserDto>(url);
        }

        public async Task<AuthUserDto?> GetUserAsync(string userId) =>
            await GetAsync<AuthUserDto>($"/api/users/{userId}");

        public async Task<(bool Success, string? Error)> CreateUserAsync(AdminCreateUserRequest request)
        {
            // Admin creates users through the public register endpoint;

            // the auto-assigned Customer role can then be supplemented via AssignRole
            var response = await PostJsonAsync("/api/auth/register", request);

            if (response is null) return (false, "Could not reach the authentication service.");

            if (response.IsSuccessStatusCode) return (true, null);

            return (false, await ExtractErrorAsync(response));
        }

        public async Task<(bool Success, string? Error)> UpdateUserAsync(
            string userId, UpdateUserApiRequest request)
        {
            var response = await PutJsonAsync($"/api/users/{userId}", request);
            if (response is null) return (false, "Could not reach the authentication service.");
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtractErrorAsync(response));
        }

        public async Task<(bool Success, string? Error)> ActivateUserAsync(string userId)
        {
            var response = await PatchAsync($"/api/users/{userId}/activate");
            if (response is null) return (false, "Could not reach the authentication service.");
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtractErrorAsync(response));
        }

        public async Task<(bool Success, string? Error)> DeactivateUserAsync(string userId)
        {
            var response = await PatchAsync($"/api/users/{userId}/deactivate");
            if (response is null) return (false, "Could not reach the authentication service.");
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtractErrorAsync(response));
        }

        // ── Role assignment ───────────────────────────────────────────────────────

        public async Task<(bool Success, string? Error, List<string> Roles)> AssignRoleAsync(
            string userId, string roleName)
        {
            var payload = new AdminRoleRequest { UserId = userId, RoleName = roleName };
            var response = await PostJsonAsync("/api/users/assign-role", payload);
            return await ParseRoleResult(response);
        }

        public async Task<(bool Success, string? Error, List<string> Roles)> RemoveRoleAsync(
            string userId, string roleName)
        {
            var payload = new AdminRoleRequest { UserId = userId, RoleName = roleName };
            var response = await PostJsonAsync("/api/users/remove-role", payload);
            return await ParseRoleResult(response);
        }

        public async Task<List<string>> GetUserRolesAsync(string userId) =>
            await GetListAsync<string>($"/api/users/{userId}/roles");

        // ── Roles ─────────────────────────────────────────────────────────────────

        public async Task<List<RoleApiDto>> GetRolesAsync() =>
            await GetListAsync<RoleApiDto>("/api/roles");

        public async Task<(bool Success, string? Error, RoleApiDto? Role)> CreateRoleAsync(
            CreateRoleApiRequest request)
        {
            var response = await PostJsonAsync("/api/roles", request);
            if (response is null) return (false, "Could not reach the authentication service.", null);
            if (!response.IsSuccessStatusCode)
                return (false, await ExtractErrorAsync(response), null);
            var role = await DeserializeAsync<RoleApiDto>(response);
            return (true, null, role);
        }

        // ── Private helpers ───────────────────────────────────────────────────────

        private async Task<List<T>> GetListAsync<T>(string url)
        {
            try
            {
                var response = await _http.GetAsync(url);
                if (!response.IsSuccessStatusCode) return new();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<T>>(json, _json) ?? new();
            }
            catch { return new(); }
        }

        private async Task<T?> GetAsync<T>(string url)
        {
            try
            {
                var response = await _http.GetAsync(url);
                if (!response.IsSuccessStatusCode) return default;
                return await DeserializeAsync<T>(response);
            }
            catch { return default; }
        }

        private async Task<HttpResponseMessage?> PostJsonAsync<T>(string url, T payload)
        {
            try
            {
                var body = new StringContent(
                    JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
                return await _http.PostAsync(url, body);
            }
            catch { return null; }
        }

        private async Task<HttpResponseMessage?> PutJsonAsync<T>(string url, T payload)
        {
            try
            {
                var body = new StringContent(
                    JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
                return await _http.PutAsync(url, body);
            }
            catch { return null; }
        }

        private async Task<HttpResponseMessage?> PatchAsync(string url)
        {
            try { return await _http.PatchAsync(url, null); }
            catch { return null; }
        }

        private async Task<T?> DeserializeAsync<T>(HttpResponseMessage response)
        {
            try
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json, _json);
            }
            catch { return default; }
        }

        private async Task<string> ExtractErrorAsync(HttpResponseMessage response)
        {
            try
            {
                var json = await response.Content.ReadAsStringAsync();
                var problem = JsonSerializer.Deserialize<AdminProblemResult>(json, _json);
                return problem?.Detail ?? problem?.Title ?? "An unexpected error occurred.";
            }
            catch { return "An unexpected error occurred."; }
        }

        private async Task<(bool, string?, List<string>)> ParseRoleResult(HttpResponseMessage? response)
        {
            if (response is null)
                return (false, "Could not reach the authentication service.", new());
            if (!response.IsSuccessStatusCode)
                return (false, await ExtractErrorAsync(response), new());
            var result = await DeserializeAsync<RoleAssignResult>(response);
            return (true, null, result?.Roles ?? new());
        }

        private sealed class AdminProblemResult
        {
            public string? Title { get; set; }
            public string? Detail { get; set; }
        }
    }

    /// <summary>Result of a role assignment or removal.</summary>
    public class RoleAssignResult
    {
        public string UserId { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}
