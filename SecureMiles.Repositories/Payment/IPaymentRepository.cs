using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SecureMiles.Models;



namespace SecureMiles.Repositories.Payment
{
    public interface IPaymentRepository
    {
        Task<int> AddPaymentAsync(SecureMiles.Models.Payment payment);
    }
}
