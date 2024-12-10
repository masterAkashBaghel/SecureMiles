

namespace SecureMiles.Models
{
    public class Payment
    {
        public int PaymentId { get; set; } // Primary Key
        public string? TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public string? Status { get; set; }
        public int UserId { get; set; } // Foreign Key to User
        public int PolicyId { get; set; } // Foreign Key to Policy
        public DateTime CreatedAt { get; set; }

        public Policy? Policy { get; set; } // Add this navigation property

    }
}