using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class UpdateMedicalProfileDto
    {
        // ليستة عشان نشيل فيها الأمراض اللي اليوزر اختارها (ضغط، سكر، إلخ)
        public List<string> ChronicDiseases { get; set; } = new List<string>();

        public string? Allergy { get; set; } // الحساسية (مثل: الفول السوداني)

        public string? BloodType { get; set; } // فصيلة الدم (A+, B-, إلخ)

        [Required(ErrorMessage = "الطول مطلوب")]
        public double Height { get; set; } // الطول بالسنتيمتر

        [Required(ErrorMessage = "الوزن مطلوب")]
        public double Weight { get; set; } // الوزن بالكيلوجرام
    }
}
