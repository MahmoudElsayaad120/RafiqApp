using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class PatientForAvailableSlotDto
    {
        public int Id { get; set; }
        public TimeSpan Time { get; set; } // "09:00 ص" (للعرض)
        //public TimeSpan RawTime { get; set; } // القيمة الفعلية للبرمجة
        public bool IsReserved { get; set; } // لو محجوز يظهر بلون رمادي وميضغطش عليه
    
    }
}
