using ECommerce.MVC.Models;
using ECommerce.MVC.Services;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;

namespace TestProject.Services
{

    public class PaymentServiceTests
    {
        private static (PaymentService Service, Mock<HttpMessageHandler> Handler) CreateService(HttpResponseMessage response)
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

            return (new PaymentService(httpClient), handler);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ReturnsPaymentResponse_WhenSuccessful()
        {
            // Arrange
            var apiResponse = new PaymentApiResponse
            {
                CardLastFour = "pay_123",
                Status = "Success"
            };

            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(apiResponse),
                    Encoding.UTF8,
                    "application/json")
            };

            var (service, _) = CreateService(httpResponse);

            var request = new PaymentApiRequest
            {
                UserId = "john@gmail.com",
                Amount = 150
            };

            // Act
            var result = await service.ProcessPaymentAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Success", result.Status);
        }

        [Fact]
        public async Task ProcessPaymentAsync_SendsPostRequest_ToCorrectEndpoint()
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

            var service = new PaymentService(httpClient);

            var request = new PaymentApiRequest
            {
                Amount = 150
            };

            // Act
            await service.ProcessPaymentAsync(request);

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.Equal(HttpMethod.Post, capturedRequest!.Method);
            Assert.Equal("/api/payments/process", capturedRequest.RequestUri!.AbsolutePath);
        }
    }
}
