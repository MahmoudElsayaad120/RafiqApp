using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Patient : BaseEntity<int>
    {
        public string userId { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }
        public string? PictureUrl { get; set; }


        // الأعمدة الجديدة الخاصة بالملف الطبي
        public string? ChronicDiseases { get; set; } // هيتسيف كـ string مفصول بفاصلة
        public string? Allergy { get; set; }
        public string? BloodType { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }

        // Navigation properties
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
