

namespace SecureMiles.Repositories.FakePayment
{
    public interface IPaymentRepository
    {
        Task<int> AddPaymentAsync(Models.Payment payment);
        Task<List<Models.Payment>> GetPaymentsByUserIdAsync(int userId);
        Task<Models.Payment> GetPaymentByIdAsync(int paymentId);
        Task<List<Models.Payment>> GetPaymentsByPolicyIdAsync(int policyId);

        Task<Models.Payment> GetPaymentByIdAsync(int paymentId, int userId);

        Task<bool> UpdatePaymentAsync(Models.Payment payment);
    }
}