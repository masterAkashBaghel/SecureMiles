using System;
using System.ComponentModel.DataAnnotations;

namespace SecureMiles.Models;

public class Policy
{
    public int PolicyID { get; set; }

    [Required(ErrorMessage = "User ID is required.")]
    public int UserID { get; set; }  // Foreign Key to User

    [Required(ErrorMessage = "Vehicle ID is required.")]
    public int VehicleID { get; set; }  // Foreign Key to Vehicle

    public int? ProposalID { get; set; }  // Foreign Key to Proposal (nullable if optional)

    [Required(ErrorMessage = "Policy Type is required.")]
    [EnumDataType(typeof(PolicyType), ErrorMessage = "Invalid Policy Type.")]
    public required string Type { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Coverage Amount must be greater than 0.")]
    public decimal CoverageAmount { get; set; }

    [Required(ErrorMessage = "Policy Status is required.")]
    [EnumDataType(typeof(PolicyStatus), ErrorMessage = "Invalid Status.")]
    public string Status { get; set; } = "Active";  // Default value


    [Range(0.01, double.MaxValue, ErrorMessage = "Premium Amount must be greater than 0.")]
    public decimal PremiumAmount { get; set; }

    [Required(ErrorMessage = "Policy Start Date is required.")]
    public DateTime PolicyStartDate { get; set; }

    [Required(ErrorMessage = "Policy End Date is required.")]
    [Compare("PolicyStartDate", ErrorMessage = "Policy End Date must be later than the Policy Start Date.")]
    public DateTime PolicyEndDate { get; set; }

    public DateTime? RenewalReminderDate { get; set; }

    [Required(ErrorMessage = "CreatedAt is required.")]
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [Required]
    public required User User { get; set; }

    [Required]
    public required Vehicle Vehicle { get; set; }

    public required Proposal Proposal { get; set; } // Navigation property for Proposal

    [Required]
    public required ICollection<Payment> Payments { get; set; }

    [Required]
    public required ICollection<Claim> Claims { get; set; }
}

public enum PolicyType
{
    Comprehensive,
    ThirdParty,
    FireAndTheft
}

public enum PolicyStatus
{
    Active,
    Expired,
    Terminated
}

