using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class CouponDto
    {
        [Required]
        public string Code { get; set; }

        [Range(1,1000)]
        public int UsageLimit { get; set; }
    }
}
