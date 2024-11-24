
using SecureMiles.Common.DTOs.Policy;

namespace SecureMiles.Services.Policy
{
    public interface IPolicyServices
    {
        Task<CreatePolicyResponseDto> CreatePolicyAsync(int userId, CreatePolicyRequestDto request);
        Task<List<PolicyResponseDto>> GetPoliciesAsync(int userId);

        Task<PolicyDetailsResponseDto> GetPolicyByIdAsync(int policyId, int userId);

        Task<bool> UpdatePolicyAsync(int policyId, int userId, UpdatePolicyRequestDto request, bool isAdmin);
        Task<RenewPolicyResponseDto> RenewPolicyAsync(int policyId, int userId, RenewPolicyRequestDto request);

    }

}
