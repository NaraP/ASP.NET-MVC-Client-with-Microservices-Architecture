namespace TestProject.Services
{
    using ECommerce.MVC.Models;
    using ECommerce.MVC.Services;
    using Moq;
    using Moq.Protected;
    using System.Net;
    using System.Text;
    using Xunit;

    public class AuthApiServiceTests
    {
        private static HttpClient CreateHttpClient(HttpStatusCode statusCode, string content)
        {
            var handler = new Mock<HttpMessageHandler>();

            handler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                });

            return new HttpClient(handler.Object)
            {
                BaseAddress = new Uri("https://test.local")
            };
        }

        [Fact]
        public async Task LoginAsync_ReturnsAuthResponse_WhenSuccessful()
        {
            var json = """
        {
            "accessToken": "token123",
            "refreshToken": "refresh123"
        }
        """;

            var http = CreateHttpClient(HttpStatusCode.OK, json);
            var service = new AuthApiService(http);

            var result = await service.LoginAsync("test@test.com", "password");

            Assert.NotNull(result);
            Assert.Equal("token123", result.AccessToken);
        }

        [Fact]
        public async Task LoginAsync_ReturnsNull_WhenRequestFails()
        {
            var http = CreateHttpClient(HttpStatusCode.BadRequest, "");
            var service = new AuthApiService(http);

            var result = await service.LoginAsync("a", "b");

            Assert.Null(result);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsSuccess_WhenApiReturnsOk()
        {
            var json = """
        {
            "accessToken": "token",
            "refreshToken": "refresh"
        }
        """;

            var http = CreateHttpClient(HttpStatusCode.OK, json);
            var service = new AuthApiService(http);

            var model = new RegisterViewModel
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                Password = "123",
                ConfirmPassword = "123"
            };

            var result = await service.RegisterAsync(model);

            Assert.True(result.Success);
            Assert.NotNull(result.Response);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsError_WhenApiReturnsProblem()
        {
            var json = """
        {
            "detail": "Email already exists"
        }
        """;

            var http = CreateHttpClient(HttpStatusCode.BadRequest, json);
            var service = new AuthApiService(http);

            var result = await service.RegisterAsync(new RegisterViewModel());

            Assert.False(result.Success);
            Assert.Equal("Email already exists", result.Error);
        }

        [Fact]
        public async Task IsEmailAvailableAsync_ReturnsTrue_WhenAvailable()
        {
            var json = """{ "available": true }""";

            var http = CreateHttpClient(HttpStatusCode.OK, json);
            var service = new AuthApiService(http);

            var result = await service.IsEmailAvailableAsync("test@test.com");

            Assert.True(result);
        }

        [Fact]
        public async Task ResetPasswordAsync_ReturnsTrue_WhenApiReturnsTrue()
        {
            var http = CreateHttpClient(HttpStatusCode.OK, "true");
            var service = new AuthApiService(http);

            var result = await service.ResetPasswordAsync(new ResetPasswordViewModel());

            Assert.True(result);
        }

        [Fact]
        public async Task GetProfileAsync_ReturnsUser_WhenSuccessful()
        {
            var json = """
        {
            "email": "user@test.com",
            "firstName": "John"
        }
        """;

            var http = CreateHttpClient(HttpStatusCode.OK, json);
            var service = new AuthApiService(http);

            var result = await service.GetProfileAsync();

            Assert.NotNull(result);
            Assert.Equal("user@test.com", result.Email);
        }
    }
}
