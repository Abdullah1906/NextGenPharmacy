using MediPOS.DB;
using MediPOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediPOS.Controllers
{
    public class ReportsController : Controller
    {
        private readonly DataContext _context;
        public ReportsController(DataContext context)
        {
            _context = context;
        }
        public IActionResult StockReport()
        {
            var products = _context.Products.ToList();

            var stockReports = products.Select(p => new StockReport
            {
                ProductName = p.ProductName,
                StockQuantity = p.Stock ?? 0,
                Price = p.Price ?? 0,
                TotalValue = (p.Stock ?? 0) * p.Price ?? 0
            }).ToList();

            var viewModel = new StockReportViewModel
            {
                StockReports = stockReports,
                Products = products
            };

            return View(viewModel);
        }

        public IActionResult SalesReport()
        {
            // Get all completed sales (delivered orders)
            var yesterday = DateTime.Today.AddDays(-1);
            var sales = _context.Sales.Where(x => x.CreatedAt >= yesterday && x.CreatedAt < DateTime.Today.AddDays(1))
            .ToList();


            var soldOrderIds = sales.Select(s => s.OrderId).ToList();


            var orderItems = _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => soldOrderIds.Contains(oi.OrderId))
                .ToList();

            // Group by product
            var salesReport = orderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g =>
                {
                    var firstProduct = g.First().Product;
                    var quantitySold = g.Sum(oi => oi.Quantity);
                    var totalSales = g.Sum(oi => oi.TotalPrice);

                    // Get all purchase records for this product
                    var productPurchases = _context.Purchases.Where(p => p.ProductId == g.Key).ToList();

                    // Calculate average purchase cost (avoid divide-by-zero)
                    decimal averageCostPrice = productPurchases.Any()
                        ? productPurchases.Sum(p => p.Price * p.Quantity) / productPurchases.Sum(p => p.Quantity)
                        : 0;

                    var cost = quantitySold * averageCostPrice;
                    var profit = totalSales - cost;

                    return new SaleReport
                    {
                        ProductName = firstProduct?.ProductName ?? "Unknown",
                        QuantitySold = quantitySold,
                        TotalSales = totalSales,
                        Profit = profit
                    };
                }).ToList();

            var viewModel = new SalesReportViewModel
            {
                SaleReports = salesReport,
                TotalSalesValue = salesReport.Sum(r => r.TotalSales),
                TotalProfit = salesReport.Sum(r => r.Profit)
            };

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult SalesReportFilter(DateTime startDate, DateTime endDate, string action)
        {
            // Get all completed sales (delivered orders)
            var sales = _context.Sales
            .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
            .ToList();


            var soldOrderIds = sales.Select(s => s.OrderId).ToList();


            var orderItems = _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => soldOrderIds.Contains(oi.OrderId))
                .ToList();

            // Group by product
            var salesReport = orderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g =>
                {
                    var firstProduct = g.First().Product;
                    var quantitySold = g.Sum(oi => oi.Quantity);
                    var totalSales = g.Sum(oi => oi.TotalPrice);

                    // Get all purchase records for this product
                    var productPurchases = _context.Purchases.Where(p => p.ProductId == g.Key).ToList();

                    // Calculate average purchase cost (avoid divide-by-zero)
                    decimal averageCostPrice = productPurchases.Any()
                        ? productPurchases.Sum(p => p.Price * p.Quantity) / productPurchases.Sum(p => p.Quantity)
                        : 0;

                    var cost = quantitySold * averageCostPrice;
                    var profit = totalSales - cost;

                    return new SaleReport
                    {
                        ProductName = firstProduct?.ProductName ?? "Unknown",
                        QuantitySold = quantitySold,
                        TotalSales = totalSales,
                        Profit = profit
                    };
                }).ToList();

            var viewModel = new SalesReportViewModel
            {
                SaleReports = salesReport,
                TotalSalesValue = salesReport.Sum(r => r.TotalSales),
                TotalProfit = salesReport.Sum(r => r.Profit),

                StartDate = startDate,   
                EndDate = endDate
            };

            if (action == "print")
            {
                
                return View("SalesReportPrint", viewModel);
            }

            // Default: show results in the same SalesReport view
            return View("SalesReport", viewModel);

        }

        //[HttpPost]
        //public IActionResult Reports(DateTime startDate, DateTime endDate)
        //{

        //}

    }
}
