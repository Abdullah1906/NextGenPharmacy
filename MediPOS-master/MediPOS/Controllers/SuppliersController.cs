using MediPOS.DB;
using MediPOS.Help;
using MediPOS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediPOS.Controllers
{
    [Authorize]
    [AuthorizeUserType(1)]
    public class SuppliersController : Controller
    {
        private readonly DataContext _context;
        public SuppliersController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var vm = new SupplierViewModel
            {
                Suppliers = _context.Suppliers.ToList()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    if (supplier.Id == 0)
                    {
                        // First, create a User
                        var user = new User
                        {
                            U_NAME =supplier.Email,
                            U_EMAIL = supplier.Email,
                            U_PHONE_NO = supplier.Phone,
                            U_PASSWORD = "localhost:7089/", // You should hash passwords and collect this from UI
                            userTypeId = 3 // Assuming '3' means Supplier
                        };

                        user.CreatedBy = User.FindFirst("Id")?.Value;
                        user.CreatedAt = DateTime.Now;
                        _context.Users.Add(user);
                        await _context.SaveChangesAsync();

                        // Set the newly created user's ID to the supplier
                        supplier.UserId = user.Id;

                        // Then create the Supplier
                        _context.Suppliers.Add(supplier);
                    }
                    else
                    {
                        _context.Suppliers.Update(supplier);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError(string.Empty, "An error occurred while saving data.");
                }
            }

            var vm = new SupplierViewModel
            {
                Supplier = supplier,
                Suppliers = _context.Suppliers.ToList()
            };
            return View("Index", vm);
        }

        public IActionResult Edit(int id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier == null) return NotFound();

            var vm = new SupplierViewModel
            {
                Supplier = supplier,
                Suppliers = _context.Suppliers.ToList()
            };
            return View("Index", vm);
        }

        public IActionResult Delete(int id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
