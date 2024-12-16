using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SecureMiles.API.Controllers.Payment;
using SecureMiles.Common.DTOs.Payment;
using SecureMiles.Services.Payment;
using System;
using System.Threading.Tasks;

namespace SecureMiles.Tests.Controllers
{
    [TestFixture]
    public class PayPalControllerTests
    {
        private Mock<IPayPalService> _mockPayPalService;
        private PayPalController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockPayPalService = new Mock<IPayPalService>();
            _controller = new PayPalController(_mockPayPalService.Object);

            // Mock HttpContext
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("localhost");

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Test]
        public async Task CreatePayment_ValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new CreatePaymentRequestDto
            {
                Amount = 100.50m,
                Currency = "USD"
            };

            var response = new CreatePaymentResponseDto
            {
                ApprovalUrl = "https://example.com/approval",
                Message = "Payment created successfully."
            };

            _mockPayPalService.Setup(s => s.CreatePaymentAsync(request.Amount, request.Currency, It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.CreatePayment(request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(response));
        }

        [Test]
        public async Task CreatePayment_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreatePaymentRequestDto(); // Missing required fields
            _controller.ModelState.AddModelError("Amount", "Amount is required.");

            // Act
            var result = await _controller.CreatePayment(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task ExecutePayment_ValidRequest_ReturnsOk()
        {
            // Arrange
            var paymentId = "PAY-12345";
            var payerId = "PAYER-67890";

            var response = new ExecutePaymentResponseDto
            {
                Status = "Success",
                TransactionId = "TXN-98765",
                Message = "Payment executed successfully."
            };

            _mockPayPalService.Setup(s => s.ExecutePaymentAsync(paymentId, payerId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.ExecutePayment(paymentId, payerId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(response));
        }

        [Test]
        public async Task ExecutePayment_InvalidPaymentId_ReturnsNotFound()
        {
            // Arrange
            var paymentId = "PAY-INVALID";
            var payerId = "PAYER-67890";

            _mockPayPalService.Setup(s => s.ExecutePaymentAsync(paymentId, payerId))
                .ThrowsAsync(new KeyNotFoundException("Payment not found."));

            // Act
            var result = await _controller.ExecutePayment(paymentId, payerId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

    }
}
