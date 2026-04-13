namespace Shared.OrderModels
{
    public class OrderItemDto
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorUrl { get; set; }
        public decimal Price { get; set; }
    }
}