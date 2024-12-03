using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureMiles.Common.DTOs.Payment
{
    public class ExecutePaymentResponseDto
    {
        public string Status { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
    }
}

