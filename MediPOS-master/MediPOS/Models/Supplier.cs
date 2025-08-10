using System.ComponentModel.DataAnnotations;

namespace MediPOS.Models
{
    public class Supplier : Base
    {

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [StringLength(100)]
        public string? CompanyName { get; set; }

        [Required]
        [StringLength(15)]
        public string? Phone { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Address { get; set; }
        public int? UserId { get; set; }
    }
    public class SupplierViewModel
    {
        public Supplier Supplier { get; set; } = new();
        public List<Supplier> Suppliers { get; set; } = new();
    }

}
