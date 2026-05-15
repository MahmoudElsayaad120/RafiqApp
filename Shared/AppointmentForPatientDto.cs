using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class AppointmentForPatientDto
    {
        // دي ال DTO  بتاعه حجوزاتي القادمه و حجوزاتي المتكمله 
        public int Id { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string AppointmentDate { get; set; } = string.Empty; // التنسيق العربي
        public string AppointmentTime { get; set; } = string.Empty; // التنسيق العربي
        public string Location { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
