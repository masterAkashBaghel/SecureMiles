
using SecureMiles.Common.DTOs.Policy;
using SecureMiles.Common.DTOs.Vehicle;
using static SecureMiles.Common.DTOs.Claims.ClaimDetailsDto;

namespace SecureMiles.Common.DTOs.Admin
{
    public class UserDetailsResponseDto
    {
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Role { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? AadhaarNumber { get; set; }
        public string? PAN { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public IEnumerable<VehicleResponseDto>? Vehicles { get; set; }
        public IEnumerable<PolicyResponseDto>? Policies { get; set; }
        public IEnumerable<ClaimResponseDto>? Claims { get; set; }
    }

}