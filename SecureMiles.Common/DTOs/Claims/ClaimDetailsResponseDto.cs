
namespace SecureMiles.Common.DTOs.Claims
{
    public class ClaimDetailsResponseDto
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

        // Associated policy details
        public PolicyDetailsDto? Policy { get; set; }

        // Associated documents
        public List<DocumentDetailsDto>? Documents { get; set; }

        public ClaimVehicleDto? Vehicle { get; set; }
    }



    public class DocumentDetailsDto
    {
        public int DocumentId { get; set; }
        public string? Type { get; set; }
        public string? FilePath { get; set; }
    }

    public class ClaimVehicleDto
    {
        public int VehicleId { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
        public string? FuelType { get; set; }
        public string? ChassisNumber { get; set; }
        public string? EngineNumber { get; set; }
    }

}