using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Services.Specifications
{
    public class DoctorAvailabilitySpec : BaseSpecifications<DoctorAvailability, int>
    {
        public DoctorAvailabilitySpec(int doctorId, DateTime date) : base(d => d.DoctorId == doctorId && d.SpecificDate.Day == date.Day)
        {

        }
    }
}
