using MediPOS.DB;
using MediPOS.Help;
using MediPOS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediPOS.Controllers
{

    [Authorize]
    [AuthorizeUserType(1)] // Only userTypeId == 1 (Admin) can access
    public class ProductsController : Controller
    {
        private readonly DataContext _context;
        public ProductsController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Category()
        {
            var viewModel = new CategoryIndexViewModel
            {
                Categories = _context.Categories.ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    _context.Categories.Add(category);
                }
                else
                {
                    _context.Categories.Update(category);
                }
                _context.SaveChanges();
                return RedirectToAction("Category");
            }

            var viewModel = new CategoryIndexViewModel
            {
                Category = category,
                Categories = _context.Categories.ToList()
            };
            return View("Category", viewModel);
        }

        public IActionResult EditCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();

            var viewModel = new CategoryIndexViewModel
            {
                Category = category,
                Categories = _context.Categories.ToList()
            };
            return View("Category", viewModel);
        }

        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }
            return RedirectToAction("Category");
        }

        //[AuthorizeUserType(1,2)]
        public IActionResult Products()
        {
            var viewModel = new ProductIndexViewModel
            {
                Products = _context.Products.Include(p => p.Category).ToList(),
                Categories = _context.Categories.ToList()
            };

            return View(viewModel);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveProducts(ProductsVM product)
        {
            if (ModelState.IsValid)
            {
                // Prepare image path
                string? imagePath = product.ExistingImageUrl;

                if (product.Image != null && product.Image.Length > 0)
                {
                    // Generate unique file name
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.Image.FileName);

                    // Define upload path
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string filePath = Path.Combine(uploadsFolder, fileName);

                    // Save file to wwwroot/uploads/products
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        product.Image.CopyTo(fileStream);
                    }

                    imagePath = "/uploads/products/" + fileName;
                }

                // Map view model to entity
                var productEntity = new Product
                {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    CategoryId = product.CategoryId,
                    Price = product.Price ?? 0,
                    Stock = product.Stock ?? 0,
                    ImageUrl = imagePath,

                    IsActive = true,
                    IsDelete = false,
                };

                if (product.Id == 0)
                {
                    productEntity.CreatedBy = User.Identity.Name; // assuming logged-in user
                    productEntity.CreatedAt = DateTime.UtcNow;
                    _context.Products.Add(productEntity);
                }

                else
                {
                    productEntity.UpdatedBy = User.Identity.Name;
                    productEntity.UpdatedAt = DateTime.UtcNow;
                    _context.Products.Update(productEntity);
                }


                _context.SaveChanges();
                return RedirectToAction("Products");
            }

            // Return view with validation errors
            var viewModel = new ProductIndexViewModel
            {
                Product = product,
                Products = _context.Products.Include(p => p.Category).ToList(),
                Categories = _context.Categories.ToList()
            };

            return View("Products", viewModel);
        }



        public IActionResult EditProducts(int id)
        {
            var product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            var productVM = new ProductsVM
            {
                Id = product.Id,
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                Price = product.Price,
                Stock = product.Stock,
                Category = product.Category,
                ExistingImageUrl = product.ImageUrl  // <-- Map existing image URL here
            };

            var viewModel = new ProductIndexViewModel
            {
                Product = productVM,
                Products = _context.Products.Include(p => p.Category).ToList(),
                Categories = _context.Categories.ToList()
            };

            return View("Products", viewModel);
        }



        public IActionResult DeleteProducts(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction("Products");
        }

    }
}
