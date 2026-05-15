using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Shared
{
    public class UpdateDoctorProfileDto
    {
        public string FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Bio { get; set; }
        public IFormFile Image { get; set; }
    }
}
