namespace SecureMiles.Common.DTOs.Admin
{
    public class AdminPolicyResponseDto
    {
        public int PolicyId { get; set; }
        public string? PolicyType { get; set; }
        public decimal CoverageAmount { get; set; }
        public decimal PremiumAmount { get; set; }
        public DateTime PolicyStartDate { get; set; }
        public DateTime PolicyEndDate { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public AdminAllUserDetailsDto? User { get; set; }
        public AdminVehicleDetailsDto? Vehicle { get; set; }
    }
    public class AdminAllUserDetailsDto
    {
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
    public class AdminVehicleDetailsDto
    {
        public int VehicleId { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public string? RegistrationNumber { get; set; }
    }

}