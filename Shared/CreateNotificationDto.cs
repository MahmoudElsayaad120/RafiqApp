using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class CreateNotificationDto
    {
        public int PatientId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; } // "Appointment", "File", "Article", "Profile"
    }
}
