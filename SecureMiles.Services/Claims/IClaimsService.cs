using SecureMiles.Common.DTOs.Claims;
using static SecureMiles.Common.DTOs.Claims.ClaimDetailsDto;

namespace SecureMiles.Services.Claims
{
    public interface IClaimService
    {
        Task<FileClaimResponseDto> FileClaimAsync(int userId, FileClaimRequestDto request);
        Task<List<ClaimResponseDto>> GetClaimsAsync(int userId);
        Task<ClaimDetailsResponseDto> GetClaimByIdAsync(int claimId, int userId);
        Task<UpdateClaimResponseDto> UpdateClaimAsync(int claimId, int userId, bool isAdmin, UpdateClaimRequestDto request);
        Task<ApproveClaimResponseDto> ApproveClaimAsync(int claimId, ApproveClaimRequestsDto request, bool isAdmin);
        Task<RejectClaimResponseDto> RejectClaimAsync(int claimId, RejectClaimRequestDto request, bool isAdmin);
        Task<DeleteClaimResponseDto> DeleteClaimAsync(int claimId, int userId);
    }
}
