using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SecureMiles.API.Controllers.Documents;
using SecureMiles.Common.DTOs.Documents;
using SecureMiles.Services.Document;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SecureMiles.Tests.Controllers
{
    [TestFixture]
    public class DocumentsControllerTests
    {
        private Mock<IDocumentService> _mockDocumentService;
        private Mock<ILogger<DocumentsController>> _mockLogger;
        private DocumentsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockDocumentService = new Mock<IDocumentService>();
            _mockLogger = new Mock<ILogger<DocumentsController>>();
            _controller = new DocumentsController(_mockDocumentService.Object, _mockLogger.Object);

            // Mock HttpContext and User Claims
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "123") // Mock UserId
            }, "mock"));

            var httpContext = new DefaultHttpContext { User = user };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Test]
        public async Task UploadDocument_ValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new UploadDocumentRequestDto
            {
                Type = "Insurance Document",
                DocumentFile = new Mock<IFormFile>().Object,
                ClaimID = 1001
            };

            var response = new UploadDocumentResponseDto
            {
                DocumentUrl = "https://example.com/documents/123",
                DocumentType = request.Type,
                UploadedAt = DateTime.UtcNow
            };

            _mockDocumentService.Setup(s => s.UploadDocumentAsync(It.IsAny<int>(), request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UploadDocument(request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(response));
        }

        [Test]
        public async Task UploadDocument_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var request = new UploadDocumentRequestDto(); // Missing required fields
            _controller.ModelState.AddModelError("Type", "Document type is required.");

            // Act
            var result = await _controller.UploadDocument(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetDocument_ValidDocumentId_ReturnsOk()
        {
            // Arrange
            var documentId = 1001;
            var response = new DocumentDetailsResponseDto
            {
                DocumentId = documentId,
                Type = "Insurance Document",
                FilePath = "/documents/1001",
                UploadedAt = DateTime.UtcNow
            };

            _mockDocumentService.Setup(s => s.GetDocumentDetailsAsync(documentId, It.IsAny<int>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetDocument(documentId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(response));
        }

        [Test]
        public async Task GetDocument_InvalidDocumentId_ReturnsNotFound()
        {
            // Arrange
            var documentId = 999;
            _mockDocumentService.Setup(s => s.GetDocumentDetailsAsync(documentId, It.IsAny<int>()))
                .ThrowsAsync(new KeyNotFoundException("Document not found."));

            // Act
            var result = await _controller.GetDocument(documentId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task DeleteDocument_ValidDocumentId_ReturnsOk()
        {
            // Arrange
            var documentId = 1001;
            _mockDocumentService.Setup(s => s.DeleteDocumentAsync(documentId, It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteDocument(documentId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            // Directly compare the property values
            var expectedMessage = new { Message = "Document deleted successfully." };
            Assert.That(okResult?.Value?.ToString(), Is.EqualTo(expectedMessage.ToString()));
        }


        [Test]
        public async Task DeleteDocument_InvalidDocumentId_ReturnsNotFound()
        {
            // Arrange
            var documentId = 999;
            _mockDocumentService.Setup(s => s.DeleteDocumentAsync(documentId, It.IsAny<int>()))
                .ThrowsAsync(new KeyNotFoundException("Document not found."));

            // Act
            var result = await _controller.DeleteDocument(documentId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }
    }
}

