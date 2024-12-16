
using Microsoft.EntityFrameworkCore;
using SecureMiles.Common.Data;
using SecureMiles.Common.DTOs.Policy;


namespace SecureMiles.Repositories.Policy
{
    public class PolicyRepository : IPolicyRepository
    {
        private readonly InsuranceContext _context;

        public PolicyRepository(InsuranceContext context)
        {
            _context = context;
        }

        public async Task<int> AddPolicyAsync(Models.Policy policy)
        {
            await _context.Policies.AddAsync(policy);
            await _context.SaveChangesAsync();
            return policy.PolicyID;
        }

        public async Task<List<Models.Policy>> GetPoliciesByUserIdAsync(int userId)
        {
            return await _context.Policies
                .Include(p => p.Vehicle) // Include related vehicle details
                .Where(p => p.UserID == userId && p.Status != "Deleted") // Exclude deleted policies
                .ToListAsync();
        }
        public async Task<Models.Policy?> GetPolicyByIdAsync(int policyId, int userId)
        {
            // Return all the details associated with the policy
            return await _context.Policies
                .Include(p => p.Vehicle)
                .Include(p => p.User)
                .Include(p => p.Claims)
                .Include(p => p.Payments)
                .FirstOrDefaultAsync(p => p.PolicyID == policyId && p.UserID == userId);
        }



        // Update the policy details
        public async Task<bool> UpdatePolicyAsync(int policyId, UpdatePolicyRequestDto policyDto)
        {
            var policy = await _context.Policies.FirstOrDefaultAsync(p => p.PolicyID == policyId);
            if (policy == null)
            {
                return false; // Policy not found
            }

            // Update only the provided fields
            if (!string.IsNullOrEmpty(policyDto.PolicyType)) policy.Type = policyDto.PolicyType;
            if (policyDto.CoverageAmount.HasValue) policy.CoverageAmount = policyDto.CoverageAmount.Value;
            if (policyDto.PremiumAmount.HasValue) policy.PremiumAmount = policyDto.PremiumAmount.Value;
            if (policyDto.PolicyStartDate.HasValue) policy.PolicyStartDate = policyDto.PolicyStartDate.Value;
            if (policyDto.PolicyEndDate.HasValue) policy.PolicyEndDate = policyDto.PolicyEndDate.Value;
            if (!string.IsNullOrEmpty(policyDto.Status)) policy.Status = policyDto.Status;

            policy.UpdatedAt = DateTime.UtcNow;

            _context.Policies.Update(policy);
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<bool> RenewPolicyAsync(int policyId, DateTime newEndDate, decimal newPremiumAmount)
        {
            var policy = await _context.Policies.FirstOrDefaultAsync(p => p.PolicyID == policyId);

            if (policy == null)
            {
                return false; // Policy not found
            }

            policy.PolicyEndDate = newEndDate;
            policy.PremiumAmount = newPremiumAmount;
            policy.Status = "Active";
            policy.UpdatedAt = DateTime.UtcNow;

            _context.Policies.Update(policy);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}