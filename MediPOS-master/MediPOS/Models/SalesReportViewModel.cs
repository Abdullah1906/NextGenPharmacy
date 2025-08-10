    namespace MediPOS.Models
{
    public class SalesReportViewModel
    {
        public List<SaleReport> SaleReports { get; set; } = new();
        public decimal TotalSalesValue { get; set; }
        public decimal TotalProfit { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class SaleReport
    {
        public string ProductName { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalSales { get; set; }
        public decimal Profit { get; set; }
    }
}
