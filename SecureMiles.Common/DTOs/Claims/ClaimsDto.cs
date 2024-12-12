using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SecureMiles.Common.DTOs.Claims
{
    using System.ComponentModel.DataAnnotations;

    public class FileClaimRequestDto
    {
        [Required(ErrorMessage = "Policy ID is required.")]
        public int PolicyId { get; set; }

        [Required(ErrorMessage = "Incident date is required.")]
        public DateTime IncidentDate { get; set; }

        [Required(ErrorMessage = "Claim description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        // documents 
        public IFormFile? DocumentFile { get; set; }

        public string? ClaimAmount { get; set; }

    }

}
