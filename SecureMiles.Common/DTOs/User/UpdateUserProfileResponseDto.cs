

namespace SecureMiles.Common.DTOs.User
{
    public class UpdateUserProfileResponseDto
    {
        public int UserId { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string ZipCode { get; set; }
        public required string Phone { get; set; }

        public DateTime DOB { get; set; }
        public required string AadhaarNumber { get; set; }
        public required string PAN { get; set; }


        public DateTime UpdatedAt { get; set; }
    }

}