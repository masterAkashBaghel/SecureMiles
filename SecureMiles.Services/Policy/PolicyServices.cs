using Microsoft.Extensions.Logging;
using SecureMiles.Common.DTOs.Policy;
using SecureMiles.Repositories.Policy;
using SecureMiles.Repositories.Vehicle;

namespace SecureMiles.Services.Policy
{
    public class PolicyService : IPolicyServices
    {
        public readonly IPolicyRepository _policyRepository;
        public readonly ILogger<PolicyService> _logger;

        public readonly IVehicleRepository _vehicleRepository;

        public PolicyService(IPolicyRepository policyRepository, ILogger<PolicyService> logger, IVehicleRepository vehicleRepository)
        {
            _policyRepository = policyRepository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _vehicleRepository = vehicleRepository;
        }

        public async Task<CreatePolicyResponseDto> CreatePolicyAsync(int userId, CreatePolicyRequestDto request)
        {
            // Fetch the vehicle entity
            var userVehicle = await _vehicleRepository.GetVehicleEntityAsync(request.VehicleId, userId);
            if (userVehicle == null || !userVehicle.IsActive)
            {
                throw new KeyNotFoundException("Vehicle not found or unauthorized access.");
            }

            // Ensure PolicyEndDate is after PolicyStartDate
            if (request.PolicyEndDate <= request.PolicyStartDate)
            {
                throw new InvalidOperationException("Policy End Date must be after the Start Date.");
            }

            // Create the Policy object
            var policy = new Models.Policy
            {
                VehicleID = request.VehicleId,
                UserID = userId,
                Type = request.PolicyType ?? throw new ArgumentNullException(nameof(request.PolicyType)),
                CoverageAmount = request.CoverageAmount,
                PremiumAmount = request.PremiumAmount,
                PolicyStartDate = request.PolicyStartDate,
                PolicyEndDate = request.PolicyEndDate,
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Proposal = null, // Optional if proposal is not required
                Payments = [], // Initialize empty collections
                Claims = [],
                RenewalReminderDate = request.PolicyEndDate.AddDays(-30),
                User = userVehicle.User, // Assign the User entity
                Vehicle = userVehicle // Assign the Vehicle entity
            };

            // Add the policy to the database
            var policyId = await _policyRepository.AddPolicyAsync(policy);

            return new CreatePolicyResponseDto
            {
                PolicyId = policyId,
                Message = "Policy created successfully."
            };
        }





        public async Task<List<PolicyResponseDto>> GetPoliciesAsync(int userId)
        {
            var policies = await _policyRepository.GetPoliciesByUserIdAsync(userId);

            return policies.Select(p => new PolicyResponseDto
            {
                PolicyId = p.PolicyID,
                PolicyType = p.Type,
                CoverageAmount = p.CoverageAmount,
                PremiumAmount = p.PremiumAmount,
                PolicyStartDate = p.PolicyStartDate,
                PolicyEndDate = p.PolicyEndDate,
                Status = p.Status,
                Vehicle = new VehicleDetailsDto
                {
                    VehicleId = p.Vehicle.VehicleID,
                    Make = p.Vehicle.Make,
                    Model = p.Vehicle.Model,
                    RegistrationNumber = p.Vehicle.RegistrationNumber,
                    VehicleType = p.Vehicle.Type
                }
            }).ToList();
        }
        public async Task<PolicyDetailsResponseDto> GetPolicyByIdAsync(int policyId, int userId)
        {
            var policy = await _policyRepository.GetPolicyByIdAsync(policyId, userId);

            if (policy == null)
            {
                throw new KeyNotFoundException("Policy not found or unauthorized access.");
            }

            return new PolicyDetailsResponseDto
            {
                PolicyId = policy.PolicyID,
                PolicyType = policy.Type,
                CoverageAmount = policy.CoverageAmount,
                PremiumAmount = policy.PremiumAmount,
                PolicyStartDate = policy.PolicyStartDate,
                PolicyEndDate = policy.PolicyEndDate,
                Status = policy.Status,
                Vehicle = new VehicleDetailsDto
                {
                    VehicleId = policy.Vehicle.VehicleID,
                    Make = policy.Vehicle.Make,
                    Model = policy.Vehicle.Model,
                    RegistrationNumber = policy.Vehicle.RegistrationNumber,
                    VehicleType = policy.Vehicle.Type
                }
            };
        }


        // Update the policy details
        public async Task<bool> UpdatePolicyAsync(int policyId, int userId, UpdatePolicyRequestDto request, bool isAdmin)
        {
            // Fetch the policy
            var policy = await _policyRepository.GetPolicyByIdAsync(policyId, userId);

            if (policy == null || (!isAdmin && policy.UserID != userId))
            {
                throw new KeyNotFoundException("Policy not found or unauthorized access.");
            }

            // Validate update rules
            if (request.PolicyEndDate.HasValue && request.PolicyStartDate.HasValue &&
                request.PolicyEndDate <= request.PolicyStartDate)
            {
                throw new InvalidOperationException("Policy End Date must be after Start Date.");
            }

            // Update the policy
            var updated = await _policyRepository.UpdatePolicyAsync(policyId, request);
            if (!updated)
            {
                throw new InvalidOperationException("Failed to update the policy.");
            }

            return updated;
        }



        // renew policy

        public async Task<RenewPolicyResponseDto> RenewPolicyAsync(int policyId, int userId, RenewPolicyRequestDto request)
        {
            // Fetch the policy
            var policy = await _policyRepository.GetPolicyByIdAsync(policyId, userId);

            if (policy == null || policy.UserID != userId)
            {
                throw new KeyNotFoundException("Policy not found or unauthorized access.");
            }

            if (policy.Status != "Active" && policy.Status != "Expired")
            {
                throw new InvalidOperationException("Only active or expired policies can be renewed.");
            }

            // Calculate the new policy end date
            var newPolicyEndDate = policy.PolicyEndDate.AddMonths(request.RenewalPeriodMonths);

            // Determine the premium amount
            var newPremiumAmount = request.PremiumAmount ?? policy.PremiumAmount;

            // Update the policy in the database
            var renewed = await _policyRepository.RenewPolicyAsync(policyId, newPolicyEndDate, newPremiumAmount);

            if (!renewed)
            {
                throw new InvalidOperationException("Failed to renew the policy.");
            }

            return new RenewPolicyResponseDto
            {
                PolicyId = policyId,
                NewPolicyEndDate = newPolicyEndDate,
                NewPremiumAmount = newPremiumAmount,
                Message = "Policy renewed successfully."
            };
        }

    }

}
