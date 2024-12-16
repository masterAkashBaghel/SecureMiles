using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SecureMiles.API.Controllers.Claims;
using SecureMiles.Common.DTOs.Claims;
using SecureMiles.Models;
using SecureMiles.Services.Claims;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SecureMiles.Tests
{
    [TestFixture]
    public class ClaimsControllerTests
    {
        private Mock<IClaimService> _mockClaimService;
        private Mock<ILogger<ClaimsController>> _mockLogger;
        private ClaimsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockClaimService = new Mock<IClaimService>();
            _mockLogger = new Mock<ILogger<ClaimsController>>();
            _controller = new ClaimsController(_mockClaimService.Object, _mockLogger.Object);

            // Mock HttpContext and User Claims
            var user = new ClaimsPrincipal(new ClaimsIdentity(new System.Security.Claims.Claim[]
            {
                new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, "123"), // Mock UserId
                new System.Security.Claims.Claim(ClaimTypes.Role, "Admin") // Mock Role
            }, "mock"));

            var httpContext = new DefaultHttpContext { User = user };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Test]
        public async Task FileClaim_ValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new FileClaimRequestDto
            {
                PolicyId = 1,
                IncidentDate = DateTime.Now.AddDays(-7),
                Description = "Accident occurred."
            };

            var response = new FileClaimResponseDto
            {
                ClaimId = 1001,
                Message = "Claim filed successfully.",
                PolicyType = "Comprehensive",
                CoverageAmount = 50000,
                PremiumAmount = 2000
            };

            _mockClaimService.Setup(s => s.FileClaimAsync(It.IsAny<int>(), request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.FileClaim(request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(response));
        }

        [Test]
        public async Task FileClaim_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new FileClaimRequestDto(); // Missing required fields
            _controller.ModelState.AddModelError("PolicyId", "Policy ID is required.");

            // Act
            var result = await _controller.FileClaim(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetClaimById_ValidClaim_ReturnsOk()
        {
            // Arrange
            var claimId = 1001;
            var userId = 123; // From the mocked User claim
            var response = new ClaimDetailsResponseDto
            {
                ClaimId = claimId,
                PolicyId = 1,
                IncidentDate = DateTime.Now.AddDays(-7),
                Description = "Accident details.",
                Status = "Approved",
                ClaimAmount = 5000,
                ApprovalDate = DateTime.Now.AddDays(-2)
            };

            _mockClaimService.Setup(s => s.GetClaimByIdAsync(claimId, userId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetClaimById(claimId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(response));
        }

        [Test]
        public async Task GetClaimById_InvalidClaim_ReturnsNotFound()
        {
            // Arrange
            var claimId = 999;
            var userId = 123; // From the mocked User claim

            _mockClaimService.Setup(s => s.GetClaimByIdAsync(claimId, userId))
                .ThrowsAsync(new KeyNotFoundException("Claim not found."));

            // Act
            var result = await _controller.GetClaimById(claimId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task ApproveClaim_ValidRequest_ReturnsOk()
        {
            // Arrange
            var claimId = 1001;
            var request = new ApproveClaimRequestDto
            {
                ApprovedAmount = 1500,
                Notes = "Verification completed."
            };

            var response = new ApproveClaimResponseDto
            {
                ClaimId = claimId,
                PolicyId = 1,
                Status = "Approved",
                ApprovedAmount = 1500,
                ApprovalDate = DateTime.Now
            };

            _mockClaimService.Setup(s => s.ApproveClaimAsync(claimId, request, true))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.ApproveClaim(claimId, request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(response));
        }

        [Test]
        public async Task RejectClaim_ValidRequest_ReturnsOk()
        {
            // Arrange
            var claimId = 1001;
            var request = new RejectClaimRequestDto
            {
                Notes = "Insufficient evidence provided."
            };

            var response = new RejectClaimResponseDto
            {
                ClaimId = claimId,
                PolicyId = 1,
                Status = "Rejected",
                Notes = request.Notes,
                RejectionDate = DateTime.Now
            };

            _mockClaimService.Setup(s => s.RejectClaimAsync(claimId, request, true))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.RejectClaim(claimId, request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(response));
        }

        [Test]
        public async Task UpdateClaim_ValidRequest_ReturnsOk()
        {
            // Arrange
            var claimId = 1001;
            var userId = 123; // From the mocked User claim
            var isAdmin = true; // From the mocked Role

            var request = new UpdateClaimRequestDto
            {
                Status = "UnderReview",
                Description = "Updated details.",
                ClaimAmount = 2500
            };

            var response = new UpdateClaimResponseDto
            {
                ClaimId = claimId,
                PolicyId = 1,
                Status = "UnderReview",
                Description = "Updated details.",
                ClaimAmount = 2500,
                UpdatedAt = DateTime.UtcNow
            };

            _mockClaimService.Setup(s => s.UpdateClaimAsync(claimId, userId, isAdmin, request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateClaim(claimId, request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(response));
        }
    }
}
