using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SecureMiles.API.Controllers.Policy;
using SecureMiles.Common.DTOs.Policy;
using SecureMiles.Services.Policy;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SecureMiles.Tests.Controllers
{
    [TestFixture]
    public class PolicyControllerTests
    {
        private Mock<IPolicyServices> _mockPolicyService;
        private Mock<ILogger<PolicyController>> _mockLogger;
        private PolicyController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockPolicyService = new Mock<IPolicyServices>();
            _mockLogger = new Mock<ILogger<PolicyController>>();
            _controller = new PolicyController(_mockPolicyService.Object, _mockLogger.Object);
        }

        private void SetUserClaims(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task CreatePolicy_ValidRequest_ReturnsOk()
        {
            // Arrange
            var userId = 1;
            SetUserClaims(userId);

            var request = new CreatePolicyRequestDto
            {
                VehicleId = 101,
                PolicyType = "Comprehensive",
                CoverageAmount = 10000m,
                PremiumAmount = 500m,
                PolicyStartDate = DateTime.Now,
                PolicyEndDate = DateTime.Now.AddYears(1)
            };

            var response = new CreatePolicyResponseDto
            {
                PolicyId = 123,
                Message = "Policy created successfully."
            };

            _mockPolicyService.Setup(s => s.CreatePolicyAsync(userId, request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.CreatePolicy(request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(response));
        }

        [Test]
        public async Task CreatePolicy_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreatePolicyRequestDto(); // Missing required fields
            _controller.ModelState.AddModelError("VehicleId", "Vehicle ID is required.");

            // Act
            var result = await _controller.CreatePolicy(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        




        [Test]
        public async Task GetPolicies_ValidUser_ReturnsOk()
        {
            // Arrange
            var userId = 1;
            SetUserClaims(userId);

            var policies = new List<Services.Policy.PolicyResponseDto>
            {
                new Services.Policy.PolicyResponseDto
                {
                    PolicyId = 123,
                    PolicyType = "Comprehensive",
                    PremiumAmount = 500m,
                    PolicyStartDate = DateTime.Now,
                    PolicyEndDate = DateTime.Now.AddYears(1),
                    Status = "Active"
                }
            };

            _mockPolicyService.Setup(s => s.GetPoliciesAsync(userId))
                .ReturnsAsync(policies);

            // Act
            var result = await _controller.GetPolicies();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(policies));
        }

        [Test]
        public async Task GetPolicies_NoPoliciesFound_ReturnsNotFound()
        {
            // Arrange
            var userId = 1;
            SetUserClaims(userId);

            _mockPolicyService.Setup(s => s.GetPoliciesAsync(userId))
                .ReturnsAsync(new List<Services.Policy.PolicyResponseDto>());

            // Act
            var result = await _controller.GetPolicies();

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetPolicyById_ValidPolicy_ReturnsOk()
        {
            // Arrange
            var userId = 1;
            SetUserClaims(userId);
            var policyId = 123;

            var policyDetails = new PolicyDetailsResponseDto
            {
                PolicyId = policyId,
                PolicyType = "Comprehensive",
                CoverageAmount = 10000m,
                PremiumAmount = 500m,
                PolicyStartDate = DateTime.Now,
                PolicyEndDate = DateTime.Now.AddYears(1),
                Status = "Active"
            };

            _mockPolicyService.Setup(s => s.GetPolicyByIdAsync(policyId, userId))
                .ReturnsAsync(policyDetails);

            // Act
            var result = await _controller.GetPolicyById(policyId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(policyDetails));
        }

        [Test]
        public async Task GetPolicyById_PolicyNotFound_ReturnsNotFound()
        {
            // Arrange
            var userId = 1;
            SetUserClaims(userId);
            var policyId = 123;

            _mockPolicyService.Setup(s => s.GetPolicyByIdAsync(policyId, userId))
                .ThrowsAsync(new KeyNotFoundException("Policy not found."));

            // Act
            var result = await _controller.GetPolicyById(policyId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }
    }
}
