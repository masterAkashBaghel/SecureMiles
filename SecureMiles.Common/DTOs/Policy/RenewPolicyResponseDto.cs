
namespace SecureMiles.Common.DTOs.Policy
{
    public class RenewPolicyResponseDto
    {
        public int PolicyId { get; set; }
        public DateTime NewPolicyEndDate { get; set; }
        public decimal NewPremiumAmount { get; set; }
        public string? Message { get; set; }
    }

}