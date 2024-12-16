

using Microsoft.EntityFrameworkCore;
using SecureMiles.Common.Data;

namespace SecureMiles.Repositories.FakePayment
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly InsuranceContext _context;

        public PaymentRepository(InsuranceContext context)
        {
            _context = context;
        }

        public async Task<int> AddPaymentAsync(Models.Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment.PaymentId;
        }

        public async Task<Models.Payment> GetPaymentByIdAsync(int paymentId)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment == null)
            {
                throw new KeyNotFoundException($"Payment with ID {paymentId} not found.");
            }

            return payment;
        }

        public async Task<List<Models.Payment>> GetPaymentsByUserIdAsync(int userId)
        {
            return await _context.Payments
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Models.Payment>> GetPaymentsByPolicyIdAsync(int policyId)
        {
            return await _context.Payments
                .Where(p => p.PolicyId == policyId)
                .ToListAsync();
        }

        public async Task<Models.Payment> GetPaymentByIdAsync(int paymentId, int userId)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId && p.UserId == userId);

            if (payment == null)
            {
                throw new KeyNotFoundException($"Payment with ID {paymentId} and UserID {userId} not found.");
            }

            return payment;
        }

        public async Task<bool> UpdatePaymentAsync(Models.Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}