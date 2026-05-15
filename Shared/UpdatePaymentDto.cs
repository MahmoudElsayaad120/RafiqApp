using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class UpdatePaymentDto
    {
        [Required]
        public string AccountHolderName { get; set; }
        [Required]
        public string AccountNumber { get; set; }
        [Required]
        public string BankName { get; set; }
        [Required]
        public string IBAN { get; set; }
        public string TransferPeriod { get; set; }
    }
}
