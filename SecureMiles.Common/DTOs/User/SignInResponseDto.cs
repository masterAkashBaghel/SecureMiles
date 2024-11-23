
namespace SecureMiles.Common.DTOs
{
    public class SignInResponseDto
    {
        public required string Token { get; set; }
        public required string Message { get; set; }

        public required int StatusCode { get; set; }
    }
}
