using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Review : BaseEntity<int>
    {
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public decimal Rate { get; set; } // من 1 لـ 5
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;



        public Doctor Doctor { get; set; } = null!;
        public virtual Patient Patient { get; set; } = null!;
    }
}
