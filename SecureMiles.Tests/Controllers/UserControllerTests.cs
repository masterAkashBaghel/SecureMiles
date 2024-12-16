using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Moq;
 
using System.Security.Claims;
using System.Threading.Tasks;
 
using Microsoft.AspNetCore.Mvc;
using SecureMiles.Services;
using SecureMiles.API.Controllers;
using SecureMiles.Common.DTOs;
using SecureMiles.Common.DTOs.User;
using Microsoft.AspNetCore.Http;

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
        var signInRequest = new SignInRequestDto
        {
            Email = "test@example.com",
            Password = "securePassword"
        };

        var signInResponse = new SignInResponseDto
        {
            Token = "valid_token",
            Message = "Login successful.",
            StatusCode = 200
        };

        _userServiceMock?
            .Setup(service => service.SignInAsync(It.Is<SignInRequestDto>(
                req => req.Email == signInRequest.Email && req.Password == signInRequest.Password)))
            .ReturnsAsync(signInResponse);

        var result = await _userController!.SignIn(signInRequest) as OkObjectResult;

        // Assert the response
        Assert.That(result, Is.Not.Null);
        Assert.That(result?.StatusCode, Is.EqualTo(200));
        Assert.That(result?.Value, Is.InstanceOf<SignInResponseDto>());

        var response = result?.Value as SignInResponseDto;
        Assert.That(response?.Token, Is.EqualTo("valid_token"));
        Assert.That(response?.Message, Is.EqualTo("Login successful."));
        Assert.That(response?.StatusCode, Is.EqualTo(200));
    }



    [Test]
    public async Task SignIn_InvalidRequest_ReturnsUnauthorized()
    {
        var signInRequest = new SignInRequestDto
        {
            Email = "test@example.com",
            Password = "wrongPassword"
        };

        _userServiceMock?
            .Setup(service => service.SignInAsync(signInRequest))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials."));

        var result = await _userController!.SignIn(signInRequest) as UnauthorizedObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result?.StatusCode, Is.EqualTo(401));
    }

    




    [Test]
    public async Task GetProfile_ValidRequest_ReturnsOk()
    {
        var userId = 1;
        var userProfileResponse = new UserProfileResponseDto
        {
            UserId = userId,
            Name = "Test User",
            Email = "test@example.com",
            Phone = "1234567890",
            Address = "123 Main St",
            City = "Test City",
            State = "Test State",
            ZipCode = "12345",
            AadhaarNumber = "1234-5678-9012",
            PAN = "ABCDE1234F"
        };

        _userServiceMock?
            .Setup(service => service.GetUserProfileAsync(userId))
            .ReturnsAsync(userProfileResponse);

        var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }));

        _userController!.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = userClaims }
        };

        var result = await _userController.GetProfile() as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result?.StatusCode, Is.EqualTo(200));
        Assert.That(result?.Value, Is.EqualTo(userProfileResponse));
    }

    [Test]
    public async Task DeleteUser_ValidUserId_ReturnsNoContent()
    {
        int userId = 1;

        _userServiceMock?
            .Setup(service => service.DeleteUserAsync(userId))
            .Returns(Task.CompletedTask);

        var result = await _userController!.DeleteUser(userId) as NoContentResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result?.StatusCode, Is.EqualTo(204));
    }

    [Test]
    public async Task ForgotPassword_ValidRequest_ReturnsOk()
    {
        var forgotPasswordRequest = new ForgotPasswordRequestDto
        {
            Email = "test@example.com"
        };

        _userServiceMock?
            .Setup(service => service.InitiatePasswordResetAsync(forgotPasswordRequest))
            .ReturnsAsync(new ForgotPasswordResponseDto
            {
                Message = "Password reset email sent successfully, please check your inbox."
            });

        var result = await _userController!.ForgotPassword(forgotPasswordRequest) as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result?.StatusCode, Is.EqualTo(200));
        Assert.That(result?.Value, Is.InstanceOf<ForgotPasswordResponseDto>());
    }

    [Test]
    public async Task ResetPassword_ValidRequest_ReturnsOk()
    {
        var resetPasswordRequest = new ResetPasswordRequestDto
        {
            Token = "valid_token",
            NewPassword = "newSecurePassword"
        };

        _userServiceMock?
            .Setup(service => service.ResetPasswordAsync(resetPasswordRequest))
            .ReturnsAsync(new ResetPasswordResponseDto
            {
                Message = "Password has been reset successfully."
            });

        var result = await _userController!.ResetPassword(resetPasswordRequest) as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result?.StatusCode, Is.EqualTo(200));
        Assert.That(result?.Value, Is.InstanceOf<ResetPasswordResponseDto>());
    }
}
