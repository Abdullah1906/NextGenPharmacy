using MediPOS.DB;
using MediPOS.Models;
using Microsoft.AspNetCore.Mvc;

namespace MediPOS.Controllers
{
    public class CustomerController : Controller
    {
        private readonly DataContext _context;
        public CustomerController(DataContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            var vm = new CustomerViewModel
            {
                Customers = _context.Customers.ToList()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(Customer customer)
        {
            if (ModelState.IsValid)
            {
                if (customer.Id == 0)
                    _context.Customers.Add(customer);
                else
                    _context.Customers.Update(customer);

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            var vm = new CustomerViewModel
            {
                Customer = customer,
                Customers = _context.Customers.ToList()
            };
            return View("Index", vm);
        }

        public IActionResult Edit(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();

            var vm = new CustomerViewModel
            {
                Customer = customer,
                Customers = _context.Customers.ToList()
            };
            return View("Index", vm);
        }

        public IActionResult Delete(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
