namespace Domain.Models.OrderModels
{
    public class OrderItem : BaseEntity<Guid>
    {
        public OrderItem()
        {
            
        }

        public OrderItem(DoctorInOrderItem doctor, decimal price)
        {
            Doctor = doctor;
            Price = price;
        }

        public DoctorInOrderItem Doctor { get; set; }
        public decimal Price { get; set; }
    }
}