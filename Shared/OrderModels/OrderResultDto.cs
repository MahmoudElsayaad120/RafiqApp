using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shared.OrderModels
{
    public class OrderResultDto
    {
        public Guid Id { get; set; }

        public string UserEmail { get; set; }
        public addressDto ShippingAddress { get; set; }
        public ICollection<OrderItemDto> orderItems { get; set; } = new List<OrderItemDto>(); // Navigational Property
    }
}
