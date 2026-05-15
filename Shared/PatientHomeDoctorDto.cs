using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class PatientHomeDoctorDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Specialization { get; set; }
        public string Address { get; set; }
        public string? ImagePath { get; set; }
        public decimal Rate { get; set; }
    }
}
