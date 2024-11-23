using System.ComponentModel.DataAnnotations;

namespace SecureMiles.Common.DTOs.Vehicle
{
    public class UpdateVehicleRequestDto
    {
        [StringLength(100, ErrorMessage = "Make cannot exceed 100 characters.")]
        public string? Make { get; set; }

        [StringLength(100, ErrorMessage = "Model cannot exceed 100 characters.")]
        public string? Model { get; set; }

        [Range(1900, int.MaxValue, ErrorMessage = "Year must be between 1900 and the current year.")]
        public int Year { get; set; }

        [StringLength(20, ErrorMessage = "Registration Number cannot exceed 20 characters.")]
        public string? RegistrationNumber { get; set; }

        [StringLength(50, ErrorMessage = "Vehicle Type cannot exceed 50 characters.")]
        public string? VehicleType { get; set; }

        [StringLength(30, ErrorMessage = "Color cannot exceed 30 characters.")]
        public string? Color { get; set; }

        [StringLength(50, ErrorMessage = "Fuel Type cannot exceed 50 characters.")]
        public string? FuelType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Market Value must be greater than or equal to 0.")]
        public decimal MarketValue { get; set; }
    }

}