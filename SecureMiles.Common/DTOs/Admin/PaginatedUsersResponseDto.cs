

namespace SecureMiles.Common.DTOs.Admin
{
    public class PaginatedUsersResponseDto
    {
        public IEnumerable<UserResponseDto>? Users { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

}