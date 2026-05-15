using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Services.Specifications
{
    public class AppointmentByDoctorIdSpecifications : BaseSpecifications<Appointment, int>
    {
        public AppointmentByDoctorIdSpecifications(int id) : base(d => d.Id == id)
        {
            AddInclude(a => a.Patient);
        }
    }
}
