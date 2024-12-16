using SecureMiles.Common.DTOs.Admin;
using SecureMiles.Models;

namespace SecureMiles.Repositories.Admin
{
    public interface IAdminRepository
    {
        Task<(IEnumerable<User>, int)> GetAllUsersAsync(int pageNumber, int pageSize);

        Task<UserDetailsResponseDto?> GetUserDetailsByIdAsync(int userId);

        Task UpdateUserRoleAsync(User user, string newRole);

        Task<User?> GetUserByIdAsync(int userId);

        Task<(IEnumerable<AdminClaimResponseDto>, int)> GetAllClaimsForReviewAsync(int pageNumber, int pageSize);

        Task<(IEnumerable<AdminPolicyResponseDto>, int)> GetAllPoliciesAsync(int pageNumber, int pageSize);

        Task<DashboardDataResponseDto> GetDashboardDataAsync();

        Task<PaginatedProposalsResponseDto> GetAllProposalsAsync(int pageNumber, int pageSize);

        Task<ProposalDetailsResponseDto?> GetProposalDetailsAsync(int proposalId);

        Task DeleteProposalAsync(int proposalId);

        Task ApproveProposalAsync(int proposalId);

        Task RejectProposalAsync(int proposalId, string reason);

        Task ApproveClaimAsync(int claimId, decimal claimAmount);
    }
}
