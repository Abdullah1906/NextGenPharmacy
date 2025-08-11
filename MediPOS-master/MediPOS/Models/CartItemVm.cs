namespace MediPOS.Models
{
    public class CartItemVm
    {
        public int Quantity { get; set; } = 1;
        public Product Product { get; set; } = new();
        public decimal TotalPrice { get; set; } 
        //public Decimal TotalPrice => Product.Price * Quantity;
    }
}
