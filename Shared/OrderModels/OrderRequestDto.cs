using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.OrderModels
{
    public class OrderRequestDto
    {
        public addressDto ShipToAddress { get; set; }
        public IEnumerable<object> OrderItems { get; set; }
        public IEnumerable<object> orderItems { get; set; }
    }
}
