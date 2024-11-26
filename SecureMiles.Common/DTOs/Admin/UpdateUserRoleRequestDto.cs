using System.ComponentModel.DataAnnotations;

namespace SecureMiles.Common.DTOs.Admin
{
    public class UpdateUserRoleRequestDto
    {
        [Required(ErrorMessage = "Role is required.")]
        [EnumDataType(typeof(UserRole), ErrorMessage = "Invalid role.")]
        public string? Role { get; set; }
    }

    public enum UserRole
    {
        Admin,
        Customer,
        Officer
    }

}