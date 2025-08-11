using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MediPOS.Models;
using MediPOS.Help;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MediPOS.DB;

namespace MediPOS.Controllers
{
    [Authorize]
    [AuthorizeUserType(1)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;

        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var startDate = DateTime.Today.AddDays(-30);

            // Get sales in last 30 days
            var sales = _context.Sales
                .Where(x => x.CreatedAt >= startDate && x.CreatedAt < DateTime.Today.AddDays(1))
                .ToList();

            var soldOrderIds = sales.Select(s => s.OrderId).ToList();

            var orderItems = _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => soldOrderIds.Contains(oi.OrderId))
                .ToList();

            var salesReport = orderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g =>
                {
                    var firstProduct = g.First().Product;
                    var quantitySold = g.Sum(oi => oi.Quantity);
                    var totalSales = g.Sum(oi => oi.TotalPrice);

                    var productPurchases = _context.Purchases
                        .Where(p => p.ProductId == g.Key)
                        .ToList();

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

            // Purchases in last 30 days
            var purchases = _context.Purchases
                .Where(p => p.CreatedAt >= startDate && p.CreatedAt < DateTime.Today.AddDays(1))
                .ToList();

            var totalPurchaseValue = purchases.Sum(p => p.Price * p.Quantity);

            // ViewModel must match the @model in Index.cshtml
            var viewModel = new DashboardVm
            {
                SaleReports = salesReport,
                TotalSalesValue = salesReport.Sum(r => r.TotalSales),
                TotalProfit = salesReport.Sum(r => r.Profit),
                TotalPurchaseValue = totalPurchaseValue
            };

            return View(viewModel); // Matches @model DashboardVm
        }

        [HttpGet]
        public IActionResult GetSalesByDate(DateTime date)
        {
            var sales = _context.Sales
                .Where(x => x.CreatedAt.Date == date.Date)
                .ToList();

            if (!sales.Any())
            {
                return Json(new { success = false, message = "No sales found for this date" });
            }

            var soldOrderIds = sales.Select(s => s.OrderId).ToList();

            var orderItems = _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => soldOrderIds.Contains(oi.OrderId))
                .ToList();

            var salesReport = orderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g =>
                {
                    var firstProduct = g.First().Product;
                    var quantitySold = g.Sum(oi => oi.Quantity);
                    var totalSales = g.Sum(oi => oi.TotalPrice);

                    var productPurchases = _context.Purchases
                        .Where(p => p.ProductId == g.Key)
                        .ToList();

                    decimal averageCostPrice = productPurchases.Any()
                        ? productPurchases.Sum(p => p.Price * p.Quantity) / productPurchases.Sum(p => p.Quantity)
                        : 0;

                    var cost = quantitySold * averageCostPrice;
                    var profit = totalSales - cost;

                    return new
                    {
                        ProductName = firstProduct?.ProductName ?? "Unknown",
                        QuantitySold = quantitySold,
                        TotalSales = totalSales,
                        Profit = profit
                    };
                }).ToList();

            return Json(new
            {
                success = true,
                date = date.ToString("yyyy-MM-dd"),
                totalSales = salesReport.Sum(r => r.TotalSales),
                totalProfit = salesReport.Sum(r => r.Profit),
                items = salesReport
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
