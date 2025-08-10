namespace MediPOS.Models
{
    public class RiderVm
    {
        public Rider Riders { get; set; } = new Rider();
        public List<Rider> RiderList { get; set; } = new List<Rider>();
    }
    public class RiderOrderVM
    {
        public Order? Order { get; set; }
        public string? RiderName { get; set; }
    }
}
