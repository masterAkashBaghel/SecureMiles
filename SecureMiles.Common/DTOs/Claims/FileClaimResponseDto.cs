
namespace SecureMiles.Common.DTOs.Claims
{
    public class FileClaimResponseDto
    {
        public int ClaimId { get; set; }
        public string? Message { get; set; }
        public string? PolicyType { get; set; }
        public decimal CoverageAmount { get; set; }
        public decimal PremiumAmount { get; set; }
    }

}