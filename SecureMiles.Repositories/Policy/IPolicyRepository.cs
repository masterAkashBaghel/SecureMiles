

using SecureMiles.Common.DTOs.Policy;

namespace SecureMiles.Repositories.Policy
{
    public interface IPolicyRepository
    {
        Task<int> AddPolicyAsync(Models.Policy policy);
        Task<List<Models.Policy>> GetPoliciesByUserIdAsync(int userId);

        Task<Models.Policy?> GetPolicyByIdAsync(int policyId, int userId);

        Task<bool> UpdatePolicyAsync(int policyId, UpdatePolicyRequestDto policyDto);

        Task<bool> RenewPolicyAsync(int policyId, DateTime newEndDate, decimal newPremiumAmount);

    }

}