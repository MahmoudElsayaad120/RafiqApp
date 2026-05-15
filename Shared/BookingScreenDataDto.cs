using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class BookingScreenDataDto
    {
        // بيانات الدكتور اللي فوق في الصورة
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public string? ImagePath { get; set; }
        public double Rate { get; set; }

        // المواعيد المتاحة لليوم المختار
        public List<PatientForAvailableSlotDto> Slots { get; set; } = new();
    }
}
