namespace SecureMiles.Services.Policy
{
    public class PolicyResponseDto
    {
        public int PolicyId { get; set; }
        public string? PolicyType { get; set; } // Comprehensive, Third-Party, etc.
        public decimal CoverageAmount { get; set; }
        public decimal PremiumAmount { get; set; }
        public DateTime PolicyStartDate { get; set; }
        public DateTime PolicyEndDate { get; set; }
        public string? Status { get; set; } // Active, Expired, etc.

        // Associated vehicle details
        public VehicleDetailsDto? Vehicle { get; set; }
    }

    public class VehicleDetailsDto
    {
        public int VehicleId { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? VehicleType { get; set; } // Car, Bike, etc.
    }


}