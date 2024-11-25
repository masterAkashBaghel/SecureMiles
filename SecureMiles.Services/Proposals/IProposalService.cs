

using SecureMiles.Common.DTOs.Proposals;

namespace SecureMiles.Services.Proposals
{
    public interface IProposalService
    {
        Task<SubmitProposalResponseDto> SubmitProposalAsync(int userId, SubmitProposalRequestDto request);
        Task<List<AllProposalResponseDto>> GetProposalsAsync(int userId);
        Task<ProposalDetailsResponseDto> GetProposalByIdAsync(int proposalId, int userId);
        Task<bool> CancelProposalAsync(int proposalId, int userId);
    }
}