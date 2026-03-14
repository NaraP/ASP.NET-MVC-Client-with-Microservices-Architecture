using ECommerce.MVC.Services;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;

namespace TestProject.Services
{
    public class InventoryServiceTests
    {
        private static HttpClient CreateHttpClient(HttpStatusCode statusCode, string content)
        {
            var handler = new Mock<HttpMessageHandler>();

            handler.Protected()
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
        public async Task GetProductsAsync_ReturnsProducts_WhenSuccessful()
        {
            var json = """
        {
            "items": [
                { "id": 1, "name": "Laptop", "price": 1200 },
                { "id": 2, "name": "Mouse", "price": 20 }
            ]
        }
        """;

            var http = CreateHttpClient(HttpStatusCode.OK, json);
            var service = new InventoryService(http);

            var result = await service.GetProductsAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Laptop", result[0].Name);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsEmpty_WhenApiFails()
        {
            var http = CreateHttpClient(HttpStatusCode.BadRequest, "");
            var service = new InventoryService(http);

            var result = await service.GetProductsAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProductAsync_ReturnsProduct_WhenSuccessful()
        {
            var json = """
        {
            "id": 10,
            "name": "Keyboard",
            "price": 50
        }
        """;

            var http = CreateHttpClient(HttpStatusCode.OK, json);
            var service = new InventoryService(http);

            var result = await service.GetProductAsync(10);

            Assert.NotNull(result);
            Assert.Equal("Keyboard", result.Name);
        }

        [Fact]
        public async Task GetProductAsync_ReturnsNull_WhenApiFails()
        {
            var http = CreateHttpClient(HttpStatusCode.NotFound, "");
            var service = new InventoryService(http);

            var result = await service.GetProductAsync(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetCategoriesAsync_ReturnsCategories_WhenSuccessful()
        {
            var json = """
        [
            "Electronics",
            "Furniture",
            "Clothing"
        ]
        """;

            var http = CreateHttpClient(HttpStatusCode.OK, json);
            var service = new InventoryService(http);

            var result = await service.GetCategoriesAsync();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains("Electronics", result);
        }

        [Fact]
        public async Task GetCategoriesAsync_ReturnsEmpty_WhenApiFails()
        {
            var http = CreateHttpClient(HttpStatusCode.InternalServerError, "");
            var service = new InventoryService(http);

            var result = await service.GetCategoriesAsync();

            Assert.Empty(result);
        }
    }
}
