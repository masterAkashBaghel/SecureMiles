
using System.Threading.Tasks;
using SecureMiles.Common.DTOs;

namespace SecureMiles.Services
{
    public interface IUserService
    {
        Task<RegisterResponseDto> RegisterUserAsync(RegisterRequestDto registerRequest);
        Task<SignInResponseDto> SignInAsync(SignInRequestDto signInRequest);
    }
}
