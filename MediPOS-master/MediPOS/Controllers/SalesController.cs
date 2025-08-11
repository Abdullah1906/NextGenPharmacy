using MediPOS.DB;
using MediPOS.Help;

using MediPOS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediPOS.Controllers
{
    [Authorize]
    [AuthorizeUserType(1)]
    public class SalesController : Controller
    {
        private readonly DataContext _context;
        public SalesController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// here i got sales  file .. here file of assign
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
             var riders = _context.Riders
            .Where(r => r.IsAvailable)
            .Select(r => new Rider
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();

            var Order = _context.Orders.Include(x => x.OrderItems).ThenInclude(oi => oi.Product)
                .Select(x => new OrderDisplayVM {


                    OrderId = x.Id,
                    CustomerName = x.CustomerName,
                    OrderDate = x.OrderDate,
                    TotalAmount = x.TotalAmount,
                    DeliveryStatus = x.DeliveryStatus,
                    OrderItems = x.OrderItems.Select(x => new OrderItemVM
                    {
                        ProductName = x.Product.ProductName,
                        UnitPrice = x.UnitPrice,
                        Quantity = x.Quantity
                    }).ToList(),
                    AvailableRiders = riders

                }).ToList(); 


            return View(Order);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Save(Sale sale)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        sale.Total = sale.Price * sale.Quantity; // Calculate total price
        //        _context.Sales.Add(sale);
        //        _context.SaveChanges();

        //        // Decrease stock for the sold product
        //        var product = _context.Products.Find(sale.ProductId);
        //        if (product != null)
        //        {
        //            product.Stock -= sale.Quantity; // Update product stock
        //            _context.SaveChanges();
        //        }

        //        return RedirectToAction("Index");
        //    }

        //    var vm = new SaleViewModel
        //    {
        //        Sale = sale,
        //        Products = _context.Products.ToList(),
        //        Sales = _context.Sales.Include(s => s.Product).ToList()
        //    };
        //    return View("Index", vm);
        //}

        public IActionResult Delete(int id)
        {
            var sale = _context.Sales.Find(id);
            if (sale != null)
            {
                _context.Sales.Remove(sale);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetProductPrice(int productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                return Json(new { price = product.Price });
            }
            return Json(new { price = 0 });
        }

        public IActionResult Edit(int id)
        {
            return View();
        }

        public IActionResult Print(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product) // ensure ProductName is loaded
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var orderVm = new OrderDisplayVM
            {
                OrderId = order.Id,
                CustomerName = order.CustomerName,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                DeliveryStatus = order.DeliveryStatus,
                OrderItems = order.OrderItems.Select(oi => new OrderItemVM
                {
                    ProductName = oi.Product.ProductName,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity
                }).ToList()
            };

            return View(orderVm);
        }





    }
}
