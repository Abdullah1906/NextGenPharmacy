using MediPOS.DB;
using MediPOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediPOS.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly DataContext _context;
        public RegistrationController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            var exist = await _context.Users
                .Where(x => x.U_NAME == model.U_NAME && x.U_EMAIL == model.U_EMAIL)
                .FirstOrDefaultAsync();

            if (exist != null)
            {
                return Json(new { success = false, message = "User already exists." });
            }
            
            model.CreatedAt = DateTime.Now;
            model.CreatedBy = model.U_EMAIL;
            model.userTypeId = 2;
            _context.Users.Add(model);
            await _context.SaveChangesAsync();


            return Json(new { success = true, message = "Registration successful." });
        }


        [HttpGet]
        public IActionResult RegisterAdmin()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> RegisterAdmin(User model)
        {
            var exist = await _context.Users
                .Where(x => x.U_NAME == model.U_NAME && x.U_EMAIL == model.U_EMAIL)
                .FirstOrDefaultAsync();

            if (exist != null)
            {
                return Json(new { success = false, message = "User already exists." });

            }
            model.CreatedAt = DateTime.Now;
            model.CreatedBy = model.U_EMAIL;
            model.userTypeId = 1;
            _context.Users.Add(model);
            await _context.SaveChangesAsync();


            return Json(new { success = true, message = "Registration successful." });

        }

    }
}
