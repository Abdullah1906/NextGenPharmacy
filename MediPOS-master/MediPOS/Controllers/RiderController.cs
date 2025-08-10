using MediPOS.DB;
using MediPOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediPOS.Controllers
{
    public class RiderController : Controller
    {
        private readonly DataContext _context;

        public RiderController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
          
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new RiderVm
            {
                RiderList = await _context.Riders.ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRider(RiderVm model)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (model.Riders.Id == 0)
                {
                    // Step 1: Create User
                    var user = new User
                    {
                        U_NAME = model.Riders.Email,
                        U_EMAIL = model.Riders.Email,
                        U_PHONE_NO = model.Riders.Phone,
                        U_PASSWORD = "localhost:7089/", 
                        userTypeId = 4 
                    };

                    user.CreatedBy = User.FindFirst("Id")?.Value;
                    user.CreatedAt = DateTime.Now;
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    // Step 2: Create Rider linked with User
                    model.Riders.UserId = user.Id;
                    _context.Riders.Add(model.Riders);
                }
                else
                {
                    _context.Riders.Update(model.Riders);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction("Create");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, "An error occurred while saving rider.");
                return View("Create", model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> EditRiders(int id)
        {
            var rider = await _context.Riders.FindAsync(id);
            if (rider == null)
            {
                return NotFound();
            }

            var model = new RiderVm
            {
                Riders = rider, // Selected rider for editing
                RiderList = await _context.Riders.ToListAsync() // All riders for the table
            };

            return View("Create", model); // Reuse the Create view for editing
        }

        public IActionResult DeleteRiders(int id)
        {
            var rider = _context.Riders.Find(id);
            if (rider != null)
            {
                _context.Riders.Remove(rider);
                _context.SaveChanges();
            }
            return RedirectToAction("Create");
        }



    }
}
