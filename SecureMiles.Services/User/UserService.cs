using SecureMiles.Models;
using SecureMiles.Repositories;
using System.Security.Cryptography;
using System.Text;
using SecureMiles.Common.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SecureMiles.Common.DTOs.User;
using SecureMiles.Services.Mail;


namespace SecureMiles.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;

        private readonly EmailService _emailService;



        public UserService(IUserRepository userRepository, IConfiguration configuration, ILogger<UserService> logger, EmailService emailService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService;

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
        public async Task<ForgotPasswordResponseDto> InitiatePasswordResetAsync(ForgotPasswordRequestDto request)
        {
            // Validate email
            if (string.IsNullOrEmpty(request.Email))
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(request.Email));
            }

            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                throw new KeyNotFoundException("User with this email does not exist.");
            }

            // Generate token
            var token = GenerateOtp();
            var expiryTime = DateTime.UtcNow.AddMinutes(15); // Token valid for 15 minutes

            // Save token to database
            var resetToken = new PasswordResetToken
            {
                UserId = user.UserID,
                Token = token,
                ExpiryTime = expiryTime
            };
            await _userRepository.AddResetTokenAsync(resetToken);

            // Send email
            var subject = "Password Reset Request";
            var body = $"Your password reset token is: {token}\nThis token will expire at {expiryTime}.";
            await _emailService.SendEmailAsync(user.Email, subject, body);

            return new ForgotPasswordResponseDto
            {
                Message = "Password reset email sent successfully, please check your inbox.",
            };
        }
        public string GenerateOtp(int length = 6)
        {
            var random = new Random();
            var otp = new char[length];
            for (int i = 0; i < length; i++)
            {
                otp[i] = (char)('0' + random.Next(0, 10));
            }
            return new string(otp);
        }
        public async Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            // Validate token
            if (string.IsNullOrEmpty(request.Token))
            {
                throw new ArgumentException("Token cannot be null or empty.", nameof(request.Token));
            }
            var tokenEntity = await _userRepository.GetValidTokenAsync(request.Token);
            if (tokenEntity == null)
            {
                throw new InvalidOperationException("Invalid or expired token.");
            }

            // Hash the new password
            if (string.IsNullOrEmpty(request.NewPassword))
            {
                throw new ArgumentException("New password cannot be null or empty.", nameof(request.NewPassword));
            }
            var hashedPassword = HashPassword(request.NewPassword);

            // Update user's password
            await _userRepository.UpdateUserPasswordAsync(tokenEntity.UserId, hashedPassword);

            // Mark token as used
            await _userRepository.MarkTokenAsUsedAsync(tokenEntity);

            return new ResetPasswordResponseDto
            {
                Message = "Password has been reset successfully."
            };
        }



    }
}
