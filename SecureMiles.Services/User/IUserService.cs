
using SecureMiles.Common.DTOs;
using SecureMiles.Common.DTOs.User;

namespace SecureMiles.Services
{
    public interface IUserService
    {
        Task<RegisterResponseDto> RegisterUserAsync(RegisterRequestDto registerRequest);
        Task<SignInResponseDto> SignInAsync(SignInRequestDto signInRequest);

        Task<UpdateUserProfileResponseDto> UpdateUserProfileAsync(int userId, UpdateUserProfileRequestDto request);

        Task<UserProfileResponseDto> GetUserProfileAsync(int userId);

        Task DeleteUserAsync(int userId);
    }
}
