using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Shared
{
    public class UploadFileDto
    {
        [Required(ErrorMessage = "يرجى اختيار ملف")]
        public IFormFile File { get; set; } // الملف الفعلي (PDF, PNG, JPG)
        public string FileName { get; set; }

        [Required(ErrorMessage = "نوع الملف مطلوب")]
        public string FileType { get; set; } // (تحاليل / اشعة / تقارير)

        public string? Notes { get; set; } // ملاحظات إضافية
    }
}
