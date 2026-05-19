using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Enums;

namespace Shared
{
    public class PaymentRequestDto
    {
        public int AppointmentId { get; set; }
        public PaymentMethod PaymentMethod { get; set; } // "Card" بناءً على اختيار العميل أو "Cash"

        // البيانات دي اختيارية لأنها مش هتيجي لو اختار "Cash"
        public string? CardHolderName { get; set; }
        public string? CardNumber { get; set; }
    }
}
