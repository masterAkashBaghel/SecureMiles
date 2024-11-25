
using System.ComponentModel.DataAnnotations;
namespace SecureMiles.Common.DTOs.Claims
{

    public class RejectClaimRequestDto
    {
        [StringLength(500, ErrorMessage = "Rejection notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }
    }

}
