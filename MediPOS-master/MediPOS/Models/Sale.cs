using System.ComponentModel.DataAnnotations;

namespace MediPOS.Models
{
    public class Sale : Base
    {

        [Required]
        public int OrderId { get; set; }


        [DataType(DataType.Currency)]
        public decimal? Total { get; set; }

        public DateTime? SaleDate { get; set; } = DateTime.Now;

    }

    public class SaleViewModel
    {
        public Sale Sale { get; set; } = new();
        public List<Sale> Sales { get; set; } = new();
        public List<Product> Products { get; set; } = new();
        public List<Order> Orders { get; set; } = new();
       
    }

}
