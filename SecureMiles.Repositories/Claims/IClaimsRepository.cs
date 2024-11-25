using SecureMiles.Common.DTOs.Claims;
using SecureMiles.Models;

namespace SecureMiles.Repositories.Claims
{
    public interface IClaimRepository
    {
        Task<int> AddClaimAsync(Claim claim);
        Task<List<Claim>> GetClaimsByUserIdAsync(int userId);
        Task<Claim?> GetClaimByIdAsync(int claimId, int userId);

        Task<UpdateClaimResponseDto> UpdateClaimAsync(int claimId, int userId, bool isAdmin, UpdateClaimRequestDto request);
        Task<ApproveClaimResponseDto> ApproveClaimAsync(int claimId, ApproveClaimRequestDto request);
        Task<Claim> RejectClaimAsync(int claimId, string notes);
    }
}
