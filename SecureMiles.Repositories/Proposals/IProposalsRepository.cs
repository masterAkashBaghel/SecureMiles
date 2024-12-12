

namespace SecureMiles.Repositories.Proposals
{
    public interface IProposalsRepository
    {
        Task<int> AddProposalAsync(Models.Proposal proposal);
        Task<List<Models.Proposal>> GetProposalsByUserIdAsync(int userId);
        Task<Models.Proposal> GetProposalByIdAsync(int proposalId);
        Task<List<Models.Proposal>> GetProposalsByVehicleIdAsync(int vehicleId);

        Task<Models.Proposal> GetProposalByIdAsync(int proposalId, int userId);
        Task<bool> CancelProposalAsync(int proposalId, int userId);


        Task<bool> UpdateProposalAsync(Models.Proposal proposal);
    }
}