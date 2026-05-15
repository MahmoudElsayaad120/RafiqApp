using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class DoctorForPatientDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public decimal Rate { get; set; }
        public decimal? Price { get; set; }

        public string Address { get; set; } = string.Empty;
        public bool IsAvailableToday { get; set; } // عشان كلمة "متوفر اليوم" اللي بالأخضر
    }
}
