using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class BookAppointmentRequestDto
    {
        public int DoctorId { get; set; }
        public int SlotId { get; set; }
        //public DateTime SelectedDate { get; set; } // التاريخ اللي اختاره من الكاليندر
        //public TimeSpan SelectedSlot { get; set; } // الساعة اللي اختارها من الـ Slots
    }
}
