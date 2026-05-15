using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
     public class CancelAppointmentDto
     {
        public int AppointmentId { get; set; }

        // إضافة اختيارية لو حابب تظهرها في صفحة "تفاصيل الحجز الملغي"
        public string? CancellationReason { get; set; }
     }
}
