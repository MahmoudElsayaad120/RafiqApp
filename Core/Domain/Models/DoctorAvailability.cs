using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class DoctorAvailability : BaseEntity<int>
    {

        //public int? Id { get; set; }
        public int? DoctorId { get; set; }

        // أضف هذا الحقل لدعم التاريخ المختار في الـ UI
        public DateOnly SpecificDate { get; set; }
        public DayOfWeek DayOfWeek { get; set; } // السبت، الأحد، إلخ
        [Required]
        public TimeSpan FromTime { get; set; }
        [Required]
        public TimeSpan ToTime { get; set;}
        public bool IsActive { get; set; } = true;
        public bool IsClosed { get; set; }


        public Doctor Doctor { get; set; } = null!;
    }
}
