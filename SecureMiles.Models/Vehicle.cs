using System;
using System.ComponentModel.DataAnnotations;

namespace SecureMiles.Models
{
    public class Vehicle
    {
        public int VehicleID { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserID { get; set; }  // Foreign Key to User

        [Required(ErrorMessage = "Vehicle Type is required.")]
        [EnumDataType(typeof(VehicleType), ErrorMessage = "Invalid Vehicle Type.")]
        public required string Type { get; set; }

        [StringLength(100, ErrorMessage = "Model cannot exceed 100 characters.")]
        public required string Model { get; set; }

        [StringLength(100, ErrorMessage = "Make cannot exceed 100 characters.")]
        public required string Make { get; set; }

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

        [StringLength(30, ErrorMessage = "Color cannot exceed 30 characters.")]
        public required string Color { get; set; }

        [StringLength(50, ErrorMessage = "Fuel Type cannot exceed 50 characters.")]
        public required string FuelType { get; set; }

        public DateTime? PurchaseDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Market Value must be greater than or equal to 0.")]
        public decimal MarketValue { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public required User User { get; set; }
        public required ICollection<Policy> Policies { get; set; }

        public required ICollection<Proposal> Proposals { get; set; }
    }

    public enum VehicleType
    {
        Car,
        Bike,
        CamperVan,
        Truck
    }
}
