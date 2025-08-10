namespace MediPOS.Models
{
    public class ShopViewModel
    {
        public List<Product>? Products { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

}
