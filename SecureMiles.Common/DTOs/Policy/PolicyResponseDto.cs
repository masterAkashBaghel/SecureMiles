namespace SecureMiles.Common.DTOs.Policy
{
    public class PolicyResponseDto
    {
        public int PolicyId { get; set; }
        public string? PolicyType { get; set; }
        public decimal PremiumAmount { get; set; }
        public DateTime PolicyStartDate { get; set; }
        public DateTime PolicyEndDate { get; set; }
        public string? Status { get; set; }
    }

}