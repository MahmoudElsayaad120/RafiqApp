using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class TransactionDto
    {
        public decimal Amount { get; set; }
        public string Date { get; set; } 
        public string Status { get; set; }
        public string Type { get; set; }
    }
}
