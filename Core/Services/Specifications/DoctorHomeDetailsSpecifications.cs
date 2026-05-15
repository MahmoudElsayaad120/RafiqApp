using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Services.Specifications
{
    public class DoctorHomeDetailsSpecifications : BaseSpecifications<Doctor,int>
    {
        public DoctorHomeDetailsSpecifications(int id) : base(d => d.Id == id)
        {
            AddInclude(d => d.Appointments.Where(a => a.Status != "Cancelled").OrderBy(a => a.Date));
            
        }
    }
}
