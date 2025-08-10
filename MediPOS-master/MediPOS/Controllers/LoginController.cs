using MediPOS.DB;
using MediPOS.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MediPOS.Controllers
{
    public class LoginController : Controller
    {
        private readonly DataContext _context;
        public LoginController(DataContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Login()
        {

            return View();
        
        }
        [HttpPost]
        public async Task<IActionResult> Login(User usr)
        {
            // Validate user credentials from DB
            var exist = await _context.Users
                .Where(x => x.U_EMAIL == usr.U_EMAIL && x.U_PASSWORD == usr.U_PASSWORD)
                .FirstOrDefaultAsync();

            if (exist != null)
            {

                //HttpContext.Session.SetString("UserName", exist.U_NAME);
                // Create claims - add whatever info you want here
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, exist.U_NAME),
                    new Claim("Id", exist.Id.ToString()),
                    new Claim("UserTypeId", exist.userTypeId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Sign in user using cookie authentication
               // await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                await HttpContext.SignInAsync(
                   CookieAuthenticationDefaults.AuthenticationScheme,
                   claimsPrincipal,
                   new AuthenticationProperties
                   {
                       IsPersistent = false, // Session-only cookie — will clear on browser close
                       
                   });

                // Redirect based on user type
                if (exist.userTypeId == 1)
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (exist.userTypeId == 3)
                {
                    return RedirectToAction("SupplierList", "S_R");
                }
                else if (exist.userTypeId == 4)
                {
                    return RedirectToAction("RiderRequ", "S_R");
                }
                else
                {
                    return RedirectToAction("Index", "Landing");
                }
            }

            ViewBag.error = "User Not Found In Database";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult>  Logout()
        {
            // Clear the entire session

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();


            return RedirectToAction("Index", "Landing");
        }
        [HttpGet]
        public async Task<IActionResult>  UnAuth()
        {


            return View();
        }


        public IActionResult ForgetPassword()
        {
            return View();

        }

        [HttpPost]

        public async Task<IActionResult> ForgetPassword(User usr)
        {
            try
            {
               

                // Check if user exists by email
                var exist = await _context.Users
                    .FirstOrDefaultAsync(x => x.U_EMAIL == usr.U_EMAIL);

                if (exist != null)
                {
                    // Update the password
                    exist.U_PASSWORD = usr.U_PASSWORD;

                    // Save changes to database
                    _context.Users.Update(exist);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Password has been updated successfully!";
                    return RedirectToAction("Login", "Login");
                }

                // User not found
                ViewBag.error = "User Not Found In Database";
                return View(usr);
            }
            catch (Exception ex)
            {
                // Log exception (recommended in real app)
                ViewBag.error = "An unexpected error occurred.";
                return View(usr);
            }
        }

    }
}
