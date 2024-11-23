using SecureMiles.Models;
using SecureMiles.Repositories;
using System.Security.Cryptography;
using System.Text;
using SecureMiles.Common.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SecureMiles.Common.DTOs.User;


namespace SecureMiles.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IConfiguration configuration, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }


        public async Task<SignInResponseDto> SignInAsync(SignInRequestDto signInRequest)
        {
            // Fetch the user by email
            var user = await _userRepository.GetUserByEmailAsync(signInRequest.Email);
            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found.", signInRequest.Email);
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            // Validate the password
            if (!VerifyPassword(signInRequest.Password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid password for user with email {Email}.", signInRequest.Email);
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            // Check if the user is active
            if (!user.IsActive)
            {
                _logger.LogWarning("User with email {Email} is not active.", signInRequest.Email);
                throw new UnauthorizedAccessException("User is not active.");
            }

            // Generate a token  
            var token = GenerateJwtToken(user);

            // Return the token and success message
            _logger.LogInformation("User with email {Email} signed in successfully.", signInRequest.Email);
            return new SignInResponseDto
            {
                Token = token,
                Message = "Login successful.",
                StatusCode = 200
            };
        }

        public virtual bool VerifyPassword(string password, string storedHash)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hashedPassword = Convert.ToBase64String(hashedBytes);
            return hashedPassword == storedHash;
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKeyString = jwtSettings["SecretKey"];
            if (string.IsNullOrEmpty(secretKeyString))
            {
                throw new InvalidOperationException("SecretKey is not configured properly.");
            }
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeyString));

            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, user.UserID.ToString()),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Email, user.Email),
                new System.Security.Claims.Claim(ClaimTypes.Role, user.Role), // Include Role
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<RegisterResponseDto> RegisterUserAsync(RegisterRequestDto registerRequest)
        {
            // Check if the email is already registered
            var existingUser = await _userRepository.GetUserByEmailAsync(registerRequest.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("A user with this email already exists.");
            }


            // Hash the password
            var hashedPassword = HashPassword(registerRequest.Password);

            // Create the User object
            var newUser = new User
            {
                Name = registerRequest.Name,
                Email = registerRequest.Email,
                Phone = registerRequest.Phone,
                PasswordHash = hashedPassword,
                Role = "Customer",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Address = "",
                City = "",
                State = "",
                ZipCode = "",
                AadhaarNumber = "",
                PAN = "",
                Vehicles = [],
                Policies = [],
                Proposals = [],
                Notifications = [],
            };

            // Add the user to the database
            await _userRepository.AddUserAsync(newUser);

            // Return success message
            _logger.LogInformation("User with email {Email} registered successfully.", registerRequest.Email);
            return new RegisterResponseDto
            {
                Success = true,
                Message = "User registered successfully.",
                StatusCode = 200
            };
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        public async Task<UpdateUserProfileResponseDto> UpdateUserProfileAsync(int userId, UpdateUserProfileRequestDto request)
        {
            // Fetch the existing user
            var existingUser = await _userRepository.GetUserByIdAsync(userId);
            if (existingUser == null)
            {
                throw new KeyNotFoundException("User not found.");
            }



            // Update allowed fields only
            existingUser.Name = request.Name;
            existingUser.Address = request.Address;
            existingUser.City = request.City;
            existingUser.State = request.State;
            existingUser.ZipCode = request.ZipCode;
            existingUser.Phone = request.Phone;
            existingUser.DOB = request.DOB;
            existingUser.AadhaarNumber = request.AadhaarNumber;
            existingUser.PAN = request.PAN;

            // Update user in the database
            var updatedUser = await _userRepository.UpdateUserProfileAsync(userId, existingUser);

            // Return response DTO
            return new UpdateUserProfileResponseDto
            {
                UserId = updatedUser.UserID,
                Name = updatedUser.Name,
                Address = updatedUser.Address,
                City = updatedUser.City,
                State = updatedUser.State,
                ZipCode = updatedUser.ZipCode,
                Phone = updatedUser.Phone,
                DOB = updatedUser.DOB,
                AadhaarNumber = updatedUser.AadhaarNumber,
                PAN = updatedUser.PAN,
                UpdatedAt = updatedUser.UpdatedAt ?? DateTime.MinValue
            };
        }

        public async Task<UserProfileResponseDto> GetUserProfileAsync(int userId)
        {
            // Fetch the user from the repository
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            // Map the user entity to a response DTO
            return new UserProfileResponseDto
            {
                UserId = user.UserID,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                City = user.City,
                State = user.State,
                ZipCode = user.ZipCode,
                AadhaarNumber = user.AadhaarNumber,
                PAN = user.PAN,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task DeleteUserAsync(int userId)
        {
            // Fetch the user
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            // Soft delete by marking the user as inactive
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            // Update the user in the database
            await _userRepository.UpdateUserAsync(user);
        }

    }
}
