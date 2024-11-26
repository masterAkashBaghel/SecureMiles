
namespace SecureMiles.Common.DTOs.Admin
{
    public class AdminClaimResponseDto
    {
        public int ClaimId { get; set; }
        public string? Status { get; set; }
        public DateTime IncidentDate { get; set; }
        public string? Description { get; set; }
        public decimal? ClaimAmount { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public AdminPolicyDetailsDto? Policy { get; set; }
        public AdminUserDetailsDto? User { get; set; }
    }

    public class AdminPolicyDetailsDto
    {
        public int PolicyId { get; set; }
        public string? PolicyType { get; set; }
        public DateTime PolicyStartDate { get; set; }
        public DateTime PolicyEndDate { get; set; }
    }

    public class AdminUserDetailsDto
    {
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
    }

}