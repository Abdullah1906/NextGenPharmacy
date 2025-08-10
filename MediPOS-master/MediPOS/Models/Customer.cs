using System.ComponentModel.DataAnnotations;

namespace MediPOS.Models
{
    public class Customer : Base
    {

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [StringLength(15)]
        public string? Phone { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        public string? Address { get; set; }
    }
    public class CustomerViewModel
    {
        public Customer Customer { get; set; } = new();
        public List<Customer> Customers { get; set; } = new();
    }

}
