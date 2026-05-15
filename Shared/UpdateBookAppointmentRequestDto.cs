using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class UpdateBookAppointmentRequestDto  //  // تعديل الحجز في ال patient
    {
        public int AppointmentId { get; set; }

        //public int DoctorId { get; set; }

        //public DateTime SelectedDate { get; set; }
        public int SlotId { get; set; } //
    }
}
