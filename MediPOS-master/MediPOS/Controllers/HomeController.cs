using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MediPOS.Models;
using MediPOS.Help;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MediPOS.DB;

namespace MediPOS.Controllers;

[Authorize]
[AuthorizeUserType(1)]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DataContext _context;

    public HomeController(ILogger<HomeController> logger,DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var yesterday = DateTime.Today.AddDays(-30);
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

        var viewModel = new DashboardVm
        {
            SaleReports = salesReport,
            TotalSalesValue = salesReport.Sum(r => r.TotalSales),
            TotalProfit = salesReport.Sum(r => r.Profit)
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public IActionResult GetMonthlySales()
    {
        var twelveMonthsAgo = DateTime.Today.AddMonths(-11);

        // Get sales in last 12 months
        var sales = _context.Sales
            .Where(s => s.CreatedAt >= new DateTime(twelveMonthsAgo.Year, twelveMonthsAgo.Month, 1))
            .ToList();

        var orderIds = sales.Select(s => s.OrderId).ToList();

        // Get order items that belong to those sales
        var orderItems = _context.OrderItems
            .Where(oi => orderIds.Contains(oi.OrderId))
            .ToList();

        // Join OrderItems with Sale to get sale date
        var monthlySales = (from oi in orderItems
                            join s in sales on oi.OrderId equals s.OrderId
                            group new { oi, s } by new { s.CreatedAt.Year, s.CreatedAt.Month } into g
                            select new
                            {
                                Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("yyyy-MM"),
                                TotalQuantity = g.Sum(x => x.oi.Quantity),
                                TotalSales = g.Sum(x => x.oi.TotalPrice)
                            })
                            .OrderBy(x => x.Month)
                            .ToList();

        return Json(monthlySales);
    }


    // New API endpoint: Monthly purchases quantities for last 12 months
    [HttpGet]
    public IActionResult GetMonthlyPurchases()
    {
        var twelveMonthsAgo = DateTime.Today.AddMonths(-11);

        var purchases = _context.Purchases
            .Where(p => p.PurchaseDate >= new DateTime(twelveMonthsAgo.Year, twelveMonthsAgo.Month, 1))
            .GroupBy(p => new { p.PurchaseDate.Value.Year, p.PurchaseDate.Value.Month })
            .Select(g => new
            {
                Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("yyyy-MM"),
                TotalQuantity = g.Sum(p => p.Quantity),
                TotalCost = g.Sum(p => p.Price * p.Quantity)
            })
            .OrderBy(x => x.Month)
            .ToList();

        return Json(purchases);
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
