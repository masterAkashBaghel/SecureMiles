using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SecureMiles.API.Controllers;
using SecureMiles.API.Controllers.Proposals;
using SecureMiles.Common.DTOs.Proposals;
using SecureMiles.Services.Proposals;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SecureMiles.Tests.Controllers
{
    [TestFixture]
    public class ProposalsControllerTests
    {
        private Mock<IProposalService> _proposalServiceMock;
        private Mock<ILogger<UserController>> _loggerMock;
        private ProposalsController _controller;

        [SetUp]
        public void SetUp()
        {
            _proposalServiceMock = new Mock<IProposalService>();
            _loggerMock = new Mock<ILogger<UserController>>();
            _controller = new ProposalsController(_proposalServiceMock.Object, _loggerMock.Object);
        }

        private void SetUserContext(string userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task SubmitProposal_ValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new SubmitProposalRequestDto { VehicleId = 1, RequestedCoverage = 5000 };
            var response = new SubmitProposalResponseDto { ProposalId = 123, Message = "Proposal submitted successfully." };
            _proposalServiceMock.Setup(s => s.SubmitProposalAsync(It.IsAny<int>(), request))
                                .ReturnsAsync(response);
            SetUserContext("1");

            // Act
            var result = await _controller.SubmitProposal(request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.StatusCode, Is.EqualTo(200));
            Assert.That(okResult?.Value, Is.EqualTo(response));
        }

        [Test]
        public async Task SubmitProposal_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("VehicleId", "Vehicle ID is required.");

            // Act
            var result = await _controller.SubmitProposal(new SubmitProposalRequestDto());

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult?.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task GetProposals_ValidUser_ReturnsOk()
        {
            // Arrange
            var proposals = new List<AllProposalResponseDto>
            {
                new AllProposalResponseDto { ProposalId = 1, VehicleId = 1, Status = "Pending", RequestedCoverage = 5000, SubmissionDate = DateTime.Now }
            };
            _proposalServiceMock.Setup(s => s.GetProposalsAsync(It.IsAny<int>()))
                                .ReturnsAsync(proposals);
            SetUserContext("1");

            // Act
            var result = await _controller.GetProposals();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.StatusCode, Is.EqualTo(200));
            Assert.That(okResult?.Value, Is.EqualTo(proposals));
        }

        [Test]
        public async Task GetProposalById_ValidRequest_ReturnsOk()
        {
            // Arrange
            var proposalDetails = new ProposalDetailsResponseDto { ProposalId = 1, RequestedCoverage = 5000, Status = "Pending", SubmissionDate = DateTime.Now };
            _proposalServiceMock.Setup(s => s.GetProposalByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
                                .ReturnsAsync(proposalDetails);
            SetUserContext("1");

            // Act
            var result = await _controller.GetProposalById(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.StatusCode, Is.EqualTo(200));
            Assert.That(okResult?.Value, Is.EqualTo(proposalDetails));
        }

        [Test]
        public async Task CancelProposal_ValidRequest_ReturnsOk()
        {
            // Arrange
            _proposalServiceMock.Setup(s => s.CancelProposalAsync(It.IsAny<int>(), It.IsAny<int>()))
                                .ReturnsAsync(true); // Ensure the mock returns a valid Task result
            SetUserContext("1");

            // Act
            var result = await _controller.CancelProposal(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.StatusCode, Is.EqualTo(200));

            // Compare the message property explicitly
            var expectedMessage = "Proposal canceled successfully.";
            var actualMessage = okResult?.Value.GetType().GetProperty("Message")?.GetValue(okResult?.Value)?.ToString();
            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
        }





        [Test]
        public async Task CancelProposal_ProposalNotFound_ReturnsNotFound()
        {
            // Arrange
            _proposalServiceMock.Setup(s => s.CancelProposalAsync(It.IsAny<int>(), It.IsAny<int>()))
                                .ThrowsAsync(new KeyNotFoundException("Proposal not found."));
            SetUserContext("1");

            // Act
            var result = await _controller.CancelProposal(1);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult?.StatusCode, Is.EqualTo(404));
        }
    }
}
