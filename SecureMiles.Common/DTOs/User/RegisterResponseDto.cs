public class RegisterResponseDto
{
    public bool Success { get; set; }
    public required string Message { get; set; }

    public int StatusCode { get; set; }
}