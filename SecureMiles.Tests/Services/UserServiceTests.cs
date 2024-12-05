using Moq;
using SecureMiles.Services;
using SecureMiles.Repositories;
using SecureMiles.Models;
using SecureMiles.Common.DTOs;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using NUnit.Framework.Legacy;

[TestFixture]
public class UserServiceTests
{
    private Mock<IUserRepository>? _userRepositoryMock;
    private Mock<IConfiguration>? _configurationMock;
    private Mock<ILogger<UserService>>? _loggerMock;

    private Mock<UserService>? _userServiceMock;
    private UserService? _userService;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _configurationMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<UserService>>();
        _userService = new UserService(_userRepositoryMock.Object, _configurationMock.Object, _loggerMock.Object, null);
        _userServiceMock = new Mock<UserService>(_userRepositoryMock.Object, _configurationMock.Object, _loggerMock.Object);

    }

    [Test]
    public async Task SignInAsync_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var email = "test@example.com";
        var password = "securePassword";
        var hashedPassword = "hashedPassword"; // This should be a hashed password in a real scenario
        var user = new User
        {
            UserID = 1,
            Email = email,
            PasswordHash = hashedPassword,
            PAN = "ABCDE1234F",
            Name = "Test User",
            Address = "Test Address",
            City = "Test City",
            State = "Test State",
            ZipCode = "123456",
            Phone = "1234567890",
            AadhaarNumber = "123456789012",
            Role = "Customer",
            Vehicles = new List<Vehicle>(),
            Policies = new List<Policy>(),
            Proposals = new List<Proposal>(),
            Notifications = new List<Notification>()
        };

        _userRepositoryMock?
            .Setup(repo => repo.GetUserByEmailAsync(email))
            .ReturnsAsync(user);

        // Mock the password verification method
        _userServiceMock?
            .Setup(service => service.VerifyPassword(password, hashedPassword))
            .Returns(true);

        // Act
        var result = await _userService.SignInAsync(new SignInRequestDto
        {
            Email = email,
            Password = password
        });

        // Assert
        ClassicAssert.IsNotNull(result);
        ClassicAssert.AreEqual(200, result.StatusCode);
        ClassicAssert.IsNotNull(result.Token);
        ClassicAssert.AreEqual("Login successful.", result.Message);
    }



    [Test]
    public void SignInAsync_InvalidCredentials_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var email = "test@example.com";
        _userRepositoryMock
            .Setup(repo => repo.GetUserByEmailAsync(email))
            .ReturnsAsync((User?)null);

        // Act & Assert
        Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await _userService.SignInAsync(new SignInRequestDto
            {
                Email = email,
                Password = "wrongPassword"
            });
        });
    }
}