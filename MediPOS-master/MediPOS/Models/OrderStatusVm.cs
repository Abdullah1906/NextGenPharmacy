namespace MediPOS.Models
{
    public class OrderStatusVm
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string? DeliveryStatus { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }

        public string? RiderName { get; set; }
        public string? RiderPhone { get; set; }
        public int Progress { get; set; } 
    }
}
