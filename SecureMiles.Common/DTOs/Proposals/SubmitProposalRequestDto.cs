
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SecureMiles.Common.DTOs.Proposals
{

    public class SubmitProposalRequestDto
    {
        [Required(ErrorMessage = "Vehicle ID is required.")]
        public int VehicleId { get; set; }

        [Required(ErrorMessage = "Requested Coverage Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Coverage Amount must be greater than 0.")]
        public decimal RequestedCoverage { get; set; }

        public string? PolicyType { get; set; }

        public decimal? PremiumAmount { get; set; }

        public IFormFile? ProposalDocument { get; set; }

    }

}