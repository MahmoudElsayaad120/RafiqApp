namespace Domain.Models.OrderModels
{
    public class DoctorInOrderItem
    {
        public DoctorInOrderItem()
        {
            
        }
        public DoctorInOrderItem(int doctorId, string doctorName, string doctorUrl)
        {
            DoctorId = doctorId;
            DoctorName = doctorName;
            DoctorUrl = doctorUrl;
        }

        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorUrl { get; set; }
    }
}