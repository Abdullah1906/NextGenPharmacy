using System.ComponentModel.DataAnnotations;

namespace MediPOS.Models
{
    public class Purchase : Base
    {

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public int Quantity { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [DataType(DataType.Currency)]
        public decimal? Total { get; set; }
        public string? PurchaseCode { get; set; }

        public DateTime? PurchaseDate { get; set; } = DateTime.Now;

        // Navigation properties
        public Supplier? Supplier { get; set; }
        public Product? Product { get; set; }
    }
    public class PurchaseViewModel
    {
        public Purchase Purchase { get; set; } = new();
        public List<Purchase> Purchases { get; set; } = new();

        public List<Supplier> Suppliers { get; set; } = new();
        public List<Product> Products { get; set; } = new();
    }


}
