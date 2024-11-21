using System;
using System.ComponentModel.DataAnnotations;

namespace SecureMiles.Models
{
    public class Notification
    {
        public int NotificationID { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserID { get; set; }  // Foreign Key to User

        [Required(ErrorMessage = "Notification Type is required.")]
        [StringLength(50, ErrorMessage = "Notification Type cannot exceed 50 characters.")]
        public required string Type { get; set; }

        [Required(ErrorMessage = "Notification Message is required.")]
        [StringLength(500, ErrorMessage = "Notification Message cannot exceed 500 characters.")]
        public required string Message { get; set; }

        [Required(ErrorMessage = "Notification Date is required.")]
        public DateTime NotificationDate { get; set; }

        // Navigation properties
        [Required]
        public required User User { get; set; }
    }
}
