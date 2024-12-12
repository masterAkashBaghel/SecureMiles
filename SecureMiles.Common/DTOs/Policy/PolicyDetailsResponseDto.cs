

using SecureMiles.Common.DTOs.Claims;
using SecureMiles.Services.Policy;

namespace SecureMiles.Common.DTOs.Policy
{
    public class PolicyDetailsResponseDto
    {
        public int PolicyId { get; set; }
        public string? PolicyType { get; set; }
        public decimal CoverageAmount { get; set; }
        public decimal PremiumAmount { get; set; }
        public DateTime PolicyStartDate { get; set; }
        public DateTime PolicyEndDate { get; set; }
        public string? Status { get; set; }

        // Associated vehicle details
        public VehicleDetailsDto? Vehicle { get; set; }

        public PolicyClaimDto? Claim { get; set; }

        public PaymentDetailsResponseDto? Payment { get; set; }

        public UserDto? User { get; set; }
    }

    public class PaymentDetailsResponseDto
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }

        public string? TransactionId { get; set; }

    }

    public class UserDto
    {
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }

    public class PolicyClaimDto
    {
        public int ClaimId { get; set; }
        public int PolicyId { get; set; }
        public DateTime IncidentDate { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public decimal? ClaimAmount { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


    }

}


