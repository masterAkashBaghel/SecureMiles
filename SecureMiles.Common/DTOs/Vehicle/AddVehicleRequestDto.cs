using System.ComponentModel.DataAnnotations;

namespace SecureMiles.Common.DTOs.Vehicle
{
    public class AddVehicleRequestDto
    {
        [Required(ErrorMessage = "Vehicle Type is required.")]
        [EnumDataType(typeof(VehicleType), ErrorMessage = "Invalid Vehicle Type.")]

        public required string Type { get; set; }

        [Required(ErrorMessage = "Model is required.")]
        [StringLength(100, ErrorMessage = "Model cannot exceed 100 characters.")]
        public required string Model { get; set; }

        [Required(ErrorMessage = "Make is required.")]
        [StringLength(100, ErrorMessage = "Make cannot exceed 100 characters.")]
        public required string Make { get; set; }

        [Required(ErrorMessage = "Year is required.")]
        [Range(1900, int.MaxValue, ErrorMessage = "Year must be between 1900 and the current year.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Registration Number is required.")]
        [StringLength(20, ErrorMessage = "Registration Number cannot exceed 20 characters.")]
        public required string RegistrationNumber { get; set; }

        [Required(ErrorMessage = "Chassis Number is required.")]
        [StringLength(20, ErrorMessage = "Chassis Number cannot exceed 20 characters.")]
        public required string ChassisNumber { get; set; }

        [Required(ErrorMessage = "Engine Number is required.")]
        [StringLength(20, ErrorMessage = "Engine Number cannot exceed 20 characters.")]
        public required string EngineNumber { get; set; }

        [Required(ErrorMessage = "Color is required.")]
        [StringLength(30, ErrorMessage = "Color cannot exceed 30 characters.")]
        public required string Color { get; set; }

        [Required(ErrorMessage = "Fuel Type is required.")]
        [StringLength(50, ErrorMessage = "Fuel Type cannot exceed 50 characters.")]
        public required string FuelType { get; set; }

        public DateTime? PurchaseDate { get; set; }  // Optional

        [Range(0, double.MaxValue, ErrorMessage = "Market Value must be greater than or equal to 0.")]
        public decimal MarketValue { get; set; }
    }

    public enum VehicleType
    {
        Car,
        Bike,
        CamperVan,
        Truck
    }
}
