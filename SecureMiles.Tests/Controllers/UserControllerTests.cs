using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecureMiles.Services;
using SecureMiles.API.Controllers;
using SecureMiles.Common.DTOs;
using NUnit.Framework.Legacy;

[TestFixture]
public class UserControllerTests
{
    private Mock<IUserService>? _userServiceMock;
    private Mock<ILogger<UserController>>? _loggerMock;
    private UserController? _userController;

    [SetUp]
    public void SetUp()
    {
        _userServiceMock = new Mock<IUserService>();
        _loggerMock = new Mock<ILogger<UserController>>();
        _userController = new UserController(_userServiceMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task SignIn_ValidRequest_ReturnsOk()
    {
        // Arrange
        var signInRequest = new SignInRequestDto
        {
            Email = "test@example.com",
            Password = "securePassword"
        };

        _userServiceMock?
            .Setup(service => service.SignInAsync(signInRequest))
            .ReturnsAsync(new SignInResponseDto
            {
                Token = "valid_token",
                Message = "Login successful.",
                StatusCode = 200
            });

        // Act
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var result = await _userController.SignIn(signInRequest) as OkObjectResult;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        // Assert
        Assert.That(result, Is.Not.Null);
        ClassicAssert.AreEqual(200, result?.StatusCode);
    }

    [Test]
    public async Task SignIn_InvalidRequest_ReturnsUnauthorized()
    {
        // Arrange
        var signInRequest = new SignInRequestDto
        {
            Email = "test@example.com",
            Password = "wrongPassword"
        };

        _userServiceMock?
            .Setup(service => service.SignInAsync(signInRequest))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials."));

        // Act
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var result = await _userController.SignIn(signInRequest) as UnauthorizedObjectResult;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        // Assert
        Assert.That(result, Is.Not.Null);
        ClassicAssert.AreEqual(200, result?.StatusCode);
    }
}