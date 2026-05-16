using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Appointment : BaseEntity<int>
    {
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled, Completed
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;


        // الحقول اللي عندك...
        public decimal? ConsultationPrice { get; set; } // أضف علامة الاستفهام ؟
        public decimal? Discount { get; set; }          // أضف علامة الاستفهام ؟

        // المجموع الكلي عدله برضه عشان م يضربش لو القيم null
        public decimal TotalAmount => (ConsultationPrice ?? 0) - (Discount ?? 0);

        // Navigation properties
        public Doctor Doctor { get; set; } = null!;
        public  Patient Patient { get; set; } = null!;
    }
}
