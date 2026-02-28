using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Patient : BaseEntity<int>
    {
        //public int Id { get; set; }
        public int UserId { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
