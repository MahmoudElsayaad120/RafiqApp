using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Coupon : BaseEntity<int>
    {
        public string Code { get; set; } // Example KJ02G62
        public int UsageLimit { get; set; } // Number Uses
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public int DoctorId { get; set; } // ربط الكوبون بالدكتور اللي انشاه
    }
}
