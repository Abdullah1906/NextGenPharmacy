namespace MediPOS.Models
{
    public class SupplierPurchase:Base
    {
        public int purchaseId { get; set; }
        public string? Status { get; set; }
        public DateTime SaleDate { get; set; } = DateTime.Now;
    }
    public class PurchaseWithStatusVM
    {
        public Purchase? Purchase { get; set; }
        public SupplierPurchase? SupplierPurchase { get; set; }
    }
}
