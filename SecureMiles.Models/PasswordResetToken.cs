

namespace SecureMiles.Models
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Reference to the user
        public string? Token { get; set; } // OTP or unique token
        public DateTime ExpiryTime { get; set; }
        public bool IsUsed { get; set; } = false; // To ensure the token is single-use
    }
}