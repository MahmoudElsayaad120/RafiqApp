using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class WalletDetailsDto
    {
        public decimal MonthlyEarnings { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal LastWithdrawal { get; set; }
        public int TransactionsCount { get; set; }

       
        public List<TransactionDto> RecentTransactions { get; set; }





    }
}
