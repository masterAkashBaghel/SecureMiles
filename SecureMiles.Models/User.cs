using System;
using System.ComponentModel.DataAnnotations;


namespace SecureMiles.Models;

public class User
{
    public int UserID { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    public required string Address { get; set; }

    [Required(ErrorMessage = "City is required.")]
    public required string City { get; set; }

    [Required(ErrorMessage = "State is required.")]
    public required string State { get; set; }

    [Required(ErrorMessage = "Zip Code is required.")]
    [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Zip Code is not valid.")]
    public required string ZipCode { get; set; }

    [Required(ErrorMessage = "Date of Birth is required.")]
    [DataType(DataType.Date)]
    public DateTime DOB { get; set; }

    [Required(ErrorMessage = "Aadhaar Number is required.")]
    [StringLength(12, MinimumLength = 12, ErrorMessage = "Aadhaar Number must be 12 digits.")]
    public required string AadhaarNumber { get; set; }

    [Required(ErrorMessage = "PAN is required.")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "PAN must be 10 characters.")]
    public required string PAN { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [Phone(ErrorMessage = "Invalid Phone Number.")]
    public required string Phone { get; set; }

    [Required(ErrorMessage = "Role is required.")]
    public required string Role { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
    public required string PasswordHash { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // One-to-many relationships

    public required ICollection<Vehicle> Vehicles { get; set; } // User owns multiple vehicles
    public required ICollection<Proposal> Proposals { get; set; } // User can submit multiple proposals
    public required ICollection<Policy> Policies { get; set; } // User can have multiple policies
    public required ICollection<Notification> Notifications { get; set; } // User can have multiple notifications
}
