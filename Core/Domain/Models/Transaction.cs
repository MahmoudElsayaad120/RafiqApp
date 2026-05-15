using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Identity;

namespace Domain.Models
{
    public class Transaction : BaseEntity<int>
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Status { get; set; }
        public string Type { get; set; }

        public string DoctorId { get; set; }

    }
}
