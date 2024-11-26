

namespace SecureMiles.Common.DTOs.Admin
{
    public class UpdateUserRoleResponseDto
    {
        public int UserId { get; set; }
        public string? OldRole { get; set; }
        public string? NewRole { get; set; }
        public string? Message { get; set; }
    }

}