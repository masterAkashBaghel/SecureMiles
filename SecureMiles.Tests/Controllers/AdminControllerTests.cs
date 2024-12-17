using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using SecureMiles.API.Controllers.Admin;
using SecureMiles.Services.Admin;
using SecureMiles.Common.DTOs.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;
using SecureMiles.Services.Claims;

namespace SecureMiles.Tests
{
    [TestFixture]
    public class AdminControllerTests
    {
        private Mock<IAdminService> _adminServiceMock;
        private Mock<IClaimService> _claimServiceMock;
        private AdminController _controller;

        [SetUp]
        public void Setup()
        {
            _adminServiceMock = new Mock<IAdminService>();
            _claimServiceMock = new Mock<IClaimService>();
            _controller = new AdminController(_adminServiceMock.Object, null, _claimServiceMock.Object); // Assuming logger is not needed for these tests
        }

        [Test]
        public async Task GetAllUsers_ShouldReturnOkResult_WhenSuccessful()
        {
            // Arrange
            var users = new PaginatedUsersResponseDto
            {
                Users = new List<UserResponseDto>
                {
                    new UserResponseDto { UserId = 1, FullName = "Test User", Email = "testuser@example.com" }
                },
                TotalCount = 1,
                CurrentPage = 1,
                PageSize = 10
            };
            _adminServiceMock.Setup(s => s.GetAllUsersAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(users);

            // Act
            var result = await _controller.GetAllUsers(1, 10);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());  // Assert that the result is of type OkObjectResult
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(users));  // Assert that the value inside the OkObjectResult is the expected users
        }

        [Test]
        public async Task GetUserDetails_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            _adminServiceMock.Setup(s => s.GetUserDetailsAsync(It.IsAny<int>())).ReturnsAsync((UserDetailsResponseDto)null);

            // Act
            var result = await _controller.GetUserDetails(1);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());  // Assert that the result is of type NotFoundObjectResult
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult?.StatusCode, Is.EqualTo(404));  // Ensure status code is 404 Not Found

            // Compare the error message property explicitly
            var expectedError = "User not found.";
            var actualError = notFoundResult?.Value.GetType().GetProperty("Error")?.GetValue(notFoundResult?.Value)?.ToString();
            Assert.That(actualError, Is.EqualTo(expectedError));
        }

        [Test]
        public async Task UpdateUserRole_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Role", "Role is required.");
            var request = new UpdateUserRoleRequestDto();

            // Act
            var result = await _controller.UpdateUserRole(1, request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());  // Assert that the result is of type BadRequestObjectResult
            var badRequestResult = result as BadRequestObjectResult;

            // Extract the ModelState errors from SerializableError
            var errors = badRequestResult?.Value as SerializableError;

            // Additional Debug Information
            if (errors == null)
            {
                Console.WriteLine("Errors dictionary is null");
            }
            else
            {
                foreach (var key in errors.Keys)
                {
                    Console.WriteLine($"Key: {key}, Value: {string.Join(", ", errors[key] as string[])}");
                }
            }

            // Ensure that the error message exists for the "Role" key
            Assert.That(errors, Is.Not.Null);
            Assert.That(errors.ContainsKey("Role"));
            var errorMessages = errors["Role"] as string[];
            Assert.That(errorMessages, Does.Contain("Role is required."));
        }

        [Test]
        public async Task GetAllClaimsForReview_ShouldReturnOkResult_WhenSuccessful()
        {
            // Arrange
            var claims = new PaginatedClaimsResponseDto
            {
                Claims = new List<AdminClaimResponseDto>
                {
                    new AdminClaimResponseDto { ClaimId = 1, Status = "Pending", IncidentDate = System.DateTime.Now }
                },
                TotalCount = 1,
                CurrentPage = 1,
                PageSize = 10
            };
            _adminServiceMock.Setup(s => s.GetAllClaimsForReviewAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(claims);

            // Act
            var result = await _controller.GetAllClaimsForReview(1, 10);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(claims));
        }

        [Test]
        public async Task GetAllPolicies_ShouldReturnOkResult_WhenSuccessful()
        {
            // Arrange
            var policies = new PaginatedPoliciesResponseDto
            {
                Policies = new List<AdminPolicyResponseDto>
                {
                    new AdminPolicyResponseDto { PolicyId = 1, PolicyType = "Health", CoverageAmount = 1000.00m }
                },
                TotalCount = 1,
                CurrentPage = 1,
                PageSize = 10
            };
            _adminServiceMock.Setup(s => s.GetAllPoliciesAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(policies);

            // Act
            var result = await _controller.GetAllPolicies(1, 10);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(policies));
        }
    }
}
