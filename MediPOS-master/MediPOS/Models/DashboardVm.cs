namespace MediPOS.Models
{
    public class DashboardVm
    {
        public List<SaleReport> SaleReports { get; set; } = new();
        public decimal TotalSalesValue { get; set; }
        public decimal TotalProfit
        {
            get; set;
        }
    }
}
