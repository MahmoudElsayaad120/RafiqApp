using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class DoctorHomeDetailsDto
    {
        public int NumberOfDayAppointments { get; set; }
        public int NumberOfWeekAppointments { get; set; }
        public decimal Rate { get; set; }
        public decimal MonthProfit { get; set; }

        public IEnumerable<AppointmentDetailsDto> ListofDayAppointments { get; set; }
    }
}
