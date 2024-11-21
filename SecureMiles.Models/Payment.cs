using System;
using System.ComponentModel.DataAnnotations;

namespace SecureMiles.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }

        [Required(ErrorMessage = "Policy ID is required.")]
        public int PolicyID { get; set; }  // Foreign Key to Policy

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment Date is required.")]
        public DateTime PaymentDate { get; set; }

        [Required(ErrorMessage = "Payment Method is required.")]
        [EnumDataType(typeof(PaymentMethod), ErrorMessage = "Invalid Payment Method.")]
        public required string PaymentMethod { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [EnumDataType(typeof(PaymentStatus), ErrorMessage = "Invalid Status.")]
        public string Status { get; set; } = "Pending";  // Default value

        [Required(ErrorMessage = "Transaction ID is required.")]
        [StringLength(50, ErrorMessage = "Transaction ID cannot exceed 50 characters.")]
        public required string TransactionID { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [Required]
        public required Policy Policy { get; set; }
    }

    public enum PaymentMethod
    {
        CreditCard,
        DebitCard,
        NetBanking,
        UPI
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed
    }
}
