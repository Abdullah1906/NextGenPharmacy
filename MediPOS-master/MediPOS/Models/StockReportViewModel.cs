namespace MediPOS.Models
{
    public class StockReportViewModel
    {
        public List<Product> Products { get; set; } = new();
        public List<StockReport> StockReports { get; set; } = new();
    }

    public class StockReport
    {
        public string ProductName { get; set; }
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalValue { get; set; }
    }

}
