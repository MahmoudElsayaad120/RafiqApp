using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Services.Specifications
{
    public class AppointmentsByDoctorAndDateSpec : BaseSpecifications<Appointment, int>
    {
        public AppointmentsByDoctorAndDateSpec(int doctorId, DateTime date)
            : base(a => a.DoctorId == doctorId && a.Date.Date == date.Date)
        {
           
        }
    }
}
