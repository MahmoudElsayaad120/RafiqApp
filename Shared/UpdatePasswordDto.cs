using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class UpdatePasswordDto
    {
        [Required(ErrorMessage = "كلمة المرور الحالية مطلوبة")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة")]
        [MinLength(8, ErrorMessage = "كلمة المرور يجب ألا تقل عن 8 أحرف")]
        // التلميح اللي في الصورة عندك (حرف كبير، رقم، إلخ) الـ Identity بيفحصها تلقائياً
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
        [Compare("NewPassword", ErrorMessage = "كلمة المرور الجديدة وغير متطابقتان")]
        public string ConfirmPassword { get; set; }
    }
}
