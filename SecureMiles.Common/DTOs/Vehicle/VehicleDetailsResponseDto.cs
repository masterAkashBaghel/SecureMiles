

using SecureMiles.Common.DTOs.Policy;
using SecureMiles.Common.DTOs.Proposals;

namespace SecureMiles.Common.DTOs.Vehicle
{
    public class VehicleDetailsResponseDto
    {
        public int VehicleId { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int Year { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? VehicleType { get; set; }
        public string? Color { get; set; }
        public string? FuelType { get; set; }
        public decimal MarketValue { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public List<ProposalResponseDto>? Proposals { get; set; }
        public List<PolicyResponseDto>? Policies { get; set; }


    }

}