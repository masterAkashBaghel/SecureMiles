using SecureMiles.Common.DTOs.Payment;

namespace SecureMiles.Services.FakePayment
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> ProcessPaymentAsync(int userId, int proposalId);
    }
}