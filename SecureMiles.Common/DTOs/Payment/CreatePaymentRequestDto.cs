using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureMiles.Common.DTOs.Payment
{
    public class CreatePaymentRequestDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
