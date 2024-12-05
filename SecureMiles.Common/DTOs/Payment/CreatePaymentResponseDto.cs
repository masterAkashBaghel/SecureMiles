using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureMiles.Common.DTOs.Payment
{
    public class CreatePaymentResponseDto
    {
        public string ApprovalUrl { get; set; }
        public string Message { get; set; }
    }
}

