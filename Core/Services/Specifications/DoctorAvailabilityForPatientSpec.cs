using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Services.Specifications
{
    public class DoctorAvailabilityForPatientSpec : BaseSpecifications<DoctorAvailability, int>
    {
        public DoctorAvailabilityForPatientSpec(int doctorId, int id)
            : base(a => a.DoctorId == doctorId && a.Id == id)
        {
        }
    }
}
