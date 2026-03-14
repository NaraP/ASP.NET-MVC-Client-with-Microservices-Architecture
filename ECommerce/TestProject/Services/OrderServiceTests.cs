using ECommerce.MVC.Models;
using ECommerce.MVC.Services;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;

namespace TestProject.Services
{
    public class OrderServiceTests
    {
        private static (OrderService Service, Mock<HttpMessageHandler> Handler) CreateService(HttpResponseMessage response)
        {
            var handler = new Mock<HttpMessageHandler>();

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var httpClient = new HttpClient(handler.Object)
            {
                BaseAddress = new Uri("https://test.local")
            };

            return (new OrderService(httpClient), handler);
        }

        [Fact]
        public async Task CreateOrderAsync_ReturnsOrderResponse_WhenSuccessful()
        {
            // Arrange
            var apiResponse = new OrderApiResponse
            {
                Status = "Created"
            };

            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(apiResponse),
                    Encoding.UTF8,
                    "application/json")
            };

            var (service, _) = CreateService(httpResponse);

            var request = new CreateOrderApiRequest
            {
                UserEmail = "John",
                TotalAmount = 100
            };

            // Act
            var result = await service.CreateOrderAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Created", result.Status);
        }

        [Fact]
        public async Task CreateOrderAsync_ReturnsNull_WhenApiFails()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);

            var (service, _) = CreateService(httpResponse);

            var request = new CreateOrderApiRequest
            {
                UserEmail = "John",
                TotalAmount = 100
            };

            // Act
            var result = await service.CreateOrderAsync(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateOrderAsync_SendsPostRequest_ToCorrectEndpoint()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;

            var handler = new Mock<HttpMessageHandler>();

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) =>
                {
                    capturedRequest = req;
                })
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{}", Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(handler.Object)
            {
                BaseAddress = new Uri("https://test.local")
            };

            var service = new OrderService(httpClient);

            var request = new CreateOrderApiRequest
            {
                UserEmail = "John",
                TotalAmount = 100
            };

            // Act
            await service.CreateOrderAsync(request);

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.Equal(HttpMethod.Post, capturedRequest!.Method);
            Assert.Equal("/api/orders", capturedRequest.RequestUri!.AbsolutePath);
        }
    }
}