using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class MedicalRecord : BaseEntity<int>
    {
        public string Title { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; } // نوع الملف (تحاليل، اشعة، تقارير) بناءً على التصفية اللي في الشاشة
        public string? Notes { get; set; } // خانة الملاحظات الاختيارية
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // تاريخ الرفع (عشان يظهر "ديسمبر 2025" زي الصورة)
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; } 
    }
}
