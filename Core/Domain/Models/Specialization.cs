using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Specialization : BaseEntity<int>
    {
        public string Name { get; set; } = string.Empty; // مثال: أسنان، باطنة، قلب
        public string? IconUrl { get; set; } = string.Empty; // مسار الأيقونة الزرقاء اللي في الصورة

        public ICollection<Doctor> Doctors { get; set; }
    }
}
