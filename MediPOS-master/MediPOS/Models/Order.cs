using System.ComponentModel.DataAnnotations;

namespace MediPOS.Models
{
    public class Order : Base
    {

        [Required]
        public string? CustomerName { get; set; }
        [Required]
        public string? Address { get; set; }
        [Required]
        public string? City { get; set; }
        [Required]
        public string? ZipCode { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Phone { get; set; }
        
        public DateTime OrderDate { get; set; }

        public decimal SubTotal { get; set; }
        public decimal Vat { get; set; }
        public decimal TotalAmount { get; set; }
        public int? RiderId { get; set; } 
        

        public DeliveryStatus DeliveryStatus { get; set; } = DeliveryStatus.Pending;
        public ICollection<OrderItem>? OrderItems { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }       
        public int ProductId { get; set; }     

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }



    /// <summary>   
    /// for delivery status and rider details

    public enum DeliveryStatus
    {
        Pending,
        Assigned,
        InTransit,
        Delivered,
        Cancelled
    }
    public class Rider
    {
        public int Id { get; set; }
        public string? Name { get; set; } 

        public string? Phone { get; set; } 

        public string? Email { get; set; }

        public string? VehicleNumber { get; set; }

        public bool IsAvailable { get; set; } = true;

        public int? UserId { get; set; }
        public ICollection<Order>? Orders { get; set; }

    }




    /// <summary>
    /// display the list to admin
    /// </summary>

    public class OrderDisplayVM
    {
        public int OrderId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
        public List<OrderItemVM> OrderItems { get; set; } = new List<OrderItemVM>();
        public List<Rider> AvailableRiders { get; set; } = new();
    }
    public class OrderItemVM
    {
        public string? ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }





}
