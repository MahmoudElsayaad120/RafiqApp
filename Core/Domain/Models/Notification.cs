using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Notification : BaseEntity<int>
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
        public string Type { get; set; }

        public string NotificationType { get; set; } = string.Empty;
        public string DoctorId { get; set; } = string.Empty;
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
    }
}
