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
    public class PurchasesController : Controller
    {
        private readonly DataContext _context;
        public PurchasesController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var vm = new PurchaseViewModel
            {
                Purchases = _context.Purchases.Include(p => p.Supplier).Include(p => p.Product).ToList(),
                Suppliers = _context.Suppliers.ToList(),
                Products = _context.Products.ToList()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(Purchase purchase)
        {
            if (ModelState.IsValid)
            {
                purchase.PurchaseCode = "PUR-" + DateTime.Now.ToString("yyyyMMddHHmmss"); // Generate unique purchase code
                purchase.Total = purchase.Price * purchase.Quantity; // Calculate total price
                purchase.CreatedAt = DateTime.Now;
                purchase.CreatedBy = User.FindFirst("Id")?.Value;
                _context.Purchases.Add(purchase);
                _context.SaveChanges();
                return RedirectToAction("Index");
                //return RedirectToAction("SupplierList", "S_R");


            }

            var vm = new PurchaseViewModel
            {
                Purchase = purchase,
                Suppliers = _context.Suppliers.ToList(),
                Products = _context.Products.ToList(),
                Purchases = _context.Purchases.Include(p => p.Supplier).Include(p => p.Product).ToList()
            };
            return View("Index", vm);
        }

        public IActionResult Delete(int id)
        {
            var purchase = _context.Purchases.Find(id);
            if (purchase != null)
            {
                _context.Purchases.Remove(purchase);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
