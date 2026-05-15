using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Payment : BaseEntity<int>
    {
        public int AppointmentId { get; set; }
        public decimal Amount { get; set; } // الـ 110 جنيه اللي في الصورة
        public string Method { get; set; } // "Card" أو "Cash"
        public string Status { get; set; } // "Pending", "Completed", "Failed"
        public string? TransactionId { get; set; } // رقم العملية المرجعي
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Appointment Appointment { get; set; } = null!;
    }
}
