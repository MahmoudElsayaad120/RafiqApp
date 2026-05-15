using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
     public class DoctorWeeklyAvailabilityDto
    {
        public string DayName { get; set; } // "السبت", "الأحد"
        public string TimeRange { get; set; } // "10:00 ص - 04:00 م"
        public bool IsClosed { get; set; } // عشان كلمة "مغلق"
    }
}
