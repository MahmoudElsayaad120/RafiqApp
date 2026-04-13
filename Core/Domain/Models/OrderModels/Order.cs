using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Identity;

namespace Domain.Models.OrderModels
{
    public class Order : BaseEntity<Guid>
    {
        public Order(string userEmail)
        {
            UserEmail = userEmail;
        }
        public Order(string userEmail, address shippingAddress, ICollection<OrderItem> orderItems)
        {
            Id = Guid.NewGuid();
            UserEmail = userEmail;
            ShippingAddress = shippingAddress;
            this.orderItems = orderItems;
        }


        public string UserEmail { get; set; }
        public address ShippingAddress { get; set; }
        public ICollection<OrderItem> orderItems { get; set; } = new List<OrderItem>(); // Navigational Property
        //public List<OrderItem> OrderItems { get; }

    }
}
