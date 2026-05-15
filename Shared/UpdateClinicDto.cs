using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class UpdateClinicDto
    {
        public string? MedicalLicense { get; set; }
        public string? ClinicName { get; set; }
        public string? Address { get; set; }
        public string? WorkHours { get; set; }
        public decimal? Price { get; set; }

    }
}
