using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class AppointmentForPatientDetailsDto
    {
        public int Id { get; set; }
        // بيانات الطبيب
        public string DoctorName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public double Rate { get; set; } 
        public int ReviewsCount { get; set; }

        // تفاصيل الموعد
        public string DayName { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        // الفاتورة
        public decimal? ConsultationPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal? TotalAmount { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
