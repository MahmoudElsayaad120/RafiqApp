using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Services.Specifications
{
    public class DoctorDetailsForPatientSpecs : BaseSpecifications<Doctor, int>
    {
        public DoctorDetailsForPatientSpecs(int id ) : base(d => d.Id == id)
        {
                AddInclude(d => d.Specialization);
                AddInclude(d => d.Reviews);
                AddInclude(d => d.Availabilities);
        }
    }
}
