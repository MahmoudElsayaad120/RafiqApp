using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ProfessionalFile : BaseEntity<int>
    {
        public string FileName { get; set; } = string.Empty; // مثلاً "شهادة التخصص"
        public string FilePath { get; set; } = string.Empty; // رابط الصورة
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;
    }
}
