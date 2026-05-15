using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class PatientSpecializationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? IconUrl { get; set; }
        public int DoctorsCount { get; set; } // الرقم اللي تحت الاسم في الصورة (مثلاً 140 طبيب)
    }
}

