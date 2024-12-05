using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SecureMiles.Common.DTOs.Payment;
using System.Threading.Tasks;


namespace SecureMiles.Services.Payment
{
    public interface IPayPalService
    {
        Task<CreatePaymentResponseDto> CreatePaymentAsync(decimal amount, string currency, string successUrl, string cancelUrl);
        Task<ExecutePaymentResponseDto> ExecutePaymentAsync(string paymentId, string payerId);
        Task<int> SavePaymentAsync(string transactionId, decimal amount, string currency, string status, int userId, int policyId);
    }
}

