namespace SecureMiles.Common.DTOs.Payment
{
    public class PaymentResponseDto
    {
        public string? TransactionId { get; set; } // Unique identifier for the transaction
        public decimal Amount { get; set; } // Amount paid
        public string? Currency { get; set; } // Currency of the payment (e.g., INR, USD)
        public string? Status { get; set; } // Payment status (e.g., Completed, Pending, Failed)
        public DateTime PaymentDate { get; set; } // Date and time of the payment

        // Policy Details
        public PolicyResponseDto? PolicyDetails { get; set; } // Includes details about the policy created

        public string? Message { get; set; } // Success message or any additional information
    }

    public class PolicyResponseDto
    {
        public int PolicyId { get; set; } // Unique identifier for the policy
        public string? PolicyType { get; set; } // Type of policy (e.g., Comprehensive, Third Party)
        public decimal CoverageAmount { get; set; } // Coverage amount of the policy
        public decimal PremiumAmount { get; set; } // Premium amount of the policy
        public DateTime PolicyStartDate { get; set; } // Start date of the policy
        public DateTime PolicyEndDate { get; set; } // End date of the policy
        public string? Status { get; set; } // Status of the policy (e.g., Active, Expired)
    }
}
