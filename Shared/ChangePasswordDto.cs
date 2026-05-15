using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "كلمه المرور الحاليه مطلوبه")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage ="كلمه المرور الجديده مطلوبه")]
        [MinLength(8,ErrorMessage ="كلمه المرور يجب ان لا تقل عن 8 احرف ")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage ="تاكيد كلمه المرور ")]
        [Compare("NewPassword",ErrorMessage ="كلمه المرور غير متطابقه")]
        public string ConfirmPassword { get; set; }
    }
}
