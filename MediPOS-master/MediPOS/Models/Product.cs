using System.ComponentModel.DataAnnotations;

namespace MediPOS.Models
{
    public class Product : Base
    {

        [Required]
        [StringLength(150)]
        public string? ProductName { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        [Display(Name = "Sell Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative.")]
        public decimal? Price { get; set; }

        [Required]
        // Add Stock property to track the stock quantity
        [Display(Name = "Stock Quantity")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
        public int? Stock { get; set; } = 1;  // New property added

        public Category? Category { get; set; }
    }

    public class ProductIndexViewModel
    {
        public ProductsVM Product { get; set; } = new ProductsVM();
        public List<Product> Products { get; set; } = new();
        public List<Category> Categories { get; set; } = new();

        public List<Product>? LatestProducts { get; set; }
    }
    public class ProductsVM : Base
    {

        [Required]
        [StringLength(150)]
        public string? ProductName { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [Display(Name = "Sell Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative.")]
        public decimal? Price { get; set; }

        [Required]
        // Add Stock property to track the stock quantity
        [Display(Name = "Stock Quantity")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
        public int? Stock { get; set; } = 1;  // New property added

        public Category? Category { get; set; }

        public IFormFile? Image { get; set; }
        public string? ExistingImageUrl { get; set; }
    }
    public class ProductIndexVm 
    {
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public string? ErrorMessage { get; set; }
    }

}
