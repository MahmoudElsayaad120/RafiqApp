using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Services.Specifications
{
    public class DoctorSpecifications :BaseSpecifications<Doctor, int>
    {
        public DoctorSpecifications(int id) : base(null)
        {
            
        }

        
    }
}
