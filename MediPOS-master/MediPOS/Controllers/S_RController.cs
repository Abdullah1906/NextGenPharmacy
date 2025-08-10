using MediPOS.DB;
using MediPOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediPOS.Controllers
{
    public class S_RController : Controller
    {
        private readonly DataContext _context;
        public S_RController(DataContext context)
        {
            _context = context;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        public async Task<IActionResult> SupplierList()
        {
            try
            {
                var userId = User.FindFirst("Id")?.Value;
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                int SupplierInt = int.Parse(userId);

                var purchases = await _context.Purchases
                     .Include(p => p.Supplier)
                     .Include(p => p.Product)
                     .Where(p => p.Supplier.UserId == SupplierInt)
                     .ToListAsync();


                var supplierPurchases = await _context.SupplierPurchases.ToListAsync();

                
                var model = purchases.Select(p => new PurchaseWithStatusVM
                {
                    Purchase = p,
                    SupplierPurchase = supplierPurchases.FirstOrDefault(sp => sp.purchaseId == p.Id)
                }).ToList();

                return View(model);

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpPost]
        public async Task<IActionResult> SaveList(int id)
        {
            try
            {
                var purchase = await _context.Purchases.FindAsync(id);
                if (purchase == null)
                {
                    return NotFound();
                }

                // Find related SupplierPurchase by purchaseId
                var supplierPurchase = await _context.SupplierPurchases
                                                     .FirstOrDefaultAsync(sp => sp.purchaseId == id);

                if (supplierPurchase != null)
                {
                    // Update existing SupplierPurchase
                    supplierPurchase.CreatedBy =User.FindFirst("Id")?.Value;
                    supplierPurchase.CreatedAt = DateTime.Now;
                    supplierPurchase.Status = "Done";
                    supplierPurchase.SaleDate = DateTime.Now;
                }
                else
                {
                    // Create new SupplierPurchase if none exists
                    var newSupplierPurchase = new SupplierPurchase
                    {

                        purchaseId = id,
                        Status = "Done",
                        SaleDate = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        CreatedBy = User.FindFirst("Id")?.Value
                    };
                    _context.SupplierPurchases.Add(newSupplierPurchase);
                }

                // Save changes to DB
                await _context.SaveChangesAsync();

                // Redirect after saving
                return RedirectToAction("SupplierList");
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<IActionResult> RiderRequ()
        {
            try
            {
                var userId = User.FindFirst("Id")?.Value;
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                int userIdInt = int.Parse(userId);

                // Get rider record for this user
                var rider = await _context.Riders.FirstOrDefaultAsync(r => r.UserId == userIdInt);
                if (rider == null) return Unauthorized();

                // Get orders and project to view model
                var orders = await _context.Orders
                    .Where(o => o.RiderId == rider.Id)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Select(o => new RiderOrderVM
                    {
                        Order = o,
                        RiderName = rider.Name ?? "N/A"
                    })
                    .ToListAsync();

                return View(orders);

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public IActionResult Print(int id)
        {
            var purchase = _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.Product)
                .FirstOrDefault(p => p.Id == id);

            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

    }
}
