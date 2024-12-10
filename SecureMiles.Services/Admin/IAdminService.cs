
using SecureMiles.Common.DTOs.Admin;

namespace SecureMiles.Services.Admin
{
    public interface IAdminService
    {
        Task<PaginatedUsersResponseDto> GetAllUsersAsync(int pageNumber, int pageSize);

        Task<UserDetailsResponseDto?> GetUserDetailsAsync(int userId);

        Task<UpdateUserRoleResponseDto> UpdateUserRoleAsync(int adminId, int userId, UpdateUserRoleRequestDto request);

        Task<PaginatedClaimsResponseDto> GetAllClaimsForReviewAsync(int pageNumber, int pageSize);

        Task<PaginatedPoliciesResponseDto> GetAllPoliciesAsync(int pageNumber, int pageSize);
        Task<DashboardDataResponseDto> GetDashboardDataAsync();

        Task<PaginatedProposalsResponseDto> GetAllProposalsAsync(int pageNumber, int pageSize);


        Task<ProposalDetailsResponseDto?> GetProposalDetailsAsync(int proposalId);


        Task<DeleteProposalResponseDto> DeleteProposalAsync(int proposalId);

        Task<ApproveProposalResponseDto> ApproveProposalAsync(int proposalId);



    }
}