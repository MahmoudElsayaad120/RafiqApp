using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Shared
{
    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "الاسم مطلوب")]
        public string Name { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public int? Age { get; set; }

        public string? Gender { get; set; }

        public IFormFile? Image { get; set; }
    }
}
