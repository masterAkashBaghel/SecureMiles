using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using SecureMiles.Common.Data;
using SecureMiles.Models;



namespace SecureMiles.Repositories.Payment
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly InsuranceContext _context;

        public PaymentRepository(InsuranceContext context)
        {
            _context = context;
        }

        public async Task<int> AddPaymentAsync(SecureMiles.Models.Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment.PaymentId;
        }
    }
}
