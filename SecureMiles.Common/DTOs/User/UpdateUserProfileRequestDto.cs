

using System.ComponentModel.DataAnnotations;

namespace SecureMiles.Common.DTOs.User
{
    public class UpdateUserProfileRequestDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string Address { get; set; }

        [Required]
        [StringLength(50)]
        public string City { get; set; }

        [Required]
        [StringLength(50)]
        public string State { get; set; }

        [Required]
        [StringLength(10)]
        public string ZipCode { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Aadhaar Number is required.")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "Aadhaar Number must be 12 digits.")]
        public required string AadhaarNumber { get; set; }

        [Required(ErrorMessage = "PAN is required.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "PAN must be 10 characters.")]
        public required string PAN { get; set; }



    }
}
