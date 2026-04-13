using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.OrderModels;

namespace Services.Specifications
{
    public class OrderSpecifications : BaseSpecifications<Order, Guid>
    {
        public OrderSpecifications(Guid id) : base(o => o.Id == id)
        {
            AddInclude(o => o.orderItems);
        }

        public OrderSpecifications(string userEmail) : base(o => o.UserEmail == userEmail)
        {
            AddInclude(o => o.orderItems);
            
        }
    }
}
