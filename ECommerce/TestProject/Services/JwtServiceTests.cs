using ECommerce.MVC.Services;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TestProject.Services
{
    public class JwtServiceTests
    {
        private static JwtService CreateService()
        {
            var configData = new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "THIS_IS_A_SUPER_SECRET_KEY_123456789",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience",
                ["Jwt:ExpiryMinutes"] = "120"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            return new JwtService(configuration);
        }

        [Fact]
        public void GenerateToken_ReturnsTokenString()
        {
            var service = CreateService();

            var token = service.GenerateToken(
                "1",
                "user@test.com",
                "John Doe",
                "Admin");

            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        [Fact]
        public void GenerateToken_ContainsExpectedClaims()
        {
            var service = CreateService();

            var tokenString = service.GenerateToken(
                "1",
                "user@test.com",
                "John Doe",
                "Admin");

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            Assert.Equal("1", token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
            Assert.Equal("user@test.com", token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
            Assert.Equal("John Doe", token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Name).Value);
            Assert.Equal("Admin", token.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        }

        [Fact]
        public void GenerateToken_HasCorrectIssuerAndAudience()
        {
            var service = CreateService();

            var tokenString = service.GenerateToken(
                "1",
                "user@test.com",
                "John Doe",
                "Admin");

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            Assert.Equal("TestIssuer", token.Issuer);
            Assert.Equal("TestAudience", token.Audiences.First());
        }

        [Fact]
        public void GenerateToken_HasExpiration()
        {
            var service = CreateService();

            var tokenString = service.GenerateToken(
                "1",
                "user@test.com",
                "John Doe",
                "Admin");

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            Assert.True(token.ValidTo > DateTime.UtcNow);
        }

        [Fact]
        public void GenerateToken_HasJtiClaim()
        {
            var service = CreateService();

            var tokenString = service.GenerateToken(
                "1",
                "user@test.com",
                "John Doe",
                "Admin");

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            var jti = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);

            Assert.NotNull(jti);
            Assert.False(string.IsNullOrWhiteSpace(jti!.Value));
        }
    }
}