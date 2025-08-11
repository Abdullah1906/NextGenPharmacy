using MediPOS.DB;
using MediPOS.Help;
using MediPOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediPOS.Controllers
{
    public class LandingController : Controller
    {
        private readonly DataContext _context;

        public LandingController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            int userTypeId = 0;

            if (User.Identity.IsAuthenticated)
            {
                var userTypeClaim = User.Claims.FirstOrDefault(c => c.Type == "UserTypeId");
                if (userTypeClaim != null)
                {
                    userTypeId = int.Parse(userTypeClaim.Value);

                   

                    if (userTypeId == 1)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    if (userTypeId == 3)
                    {
                        return RedirectToAction("SupplierList", "S_R");
                    }
                    if (userTypeId == 4)
                    {
                        return RedirectToAction("RiderRequ", "S_R");
                    }
                    //else
                    //{
                    //    return RedirectToAction("Index", "Landing");
                    //}


                }              
            }

            var latestProducts = _context.Products
                .OrderByDescending(p => p.CreatedAt)
                .Take(4) // Show latest 10 products, adjust as needed
                .ToList();
            var viewModel = new ProductIndexViewModel
            {
                Products = _context.Products
                .Include(p => p.Category)
                .Take(9) // ← This limits to 9
                .ToList(),
                Categories = _context.Categories.ToList(),
                LatestProducts = latestProducts
            };

            return View(viewModel);
        }
        public IActionResult About()
        {
            return View();
        }


        // shop with pagination
        public async Task<IActionResult> Shop(int page = 1, int pageSize = 9, string sortOrder = "", int? minPrice = null, int? maxPrice = null)
        {
            var productsQuery = _context.Products.AsQueryable();

            if (minPrice.HasValue && maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= minPrice && p.Price <= maxPrice);
            }

            // Sorting logic
            switch (sortOrder)
            {
                case "name_asc":
                    productsQuery = productsQuery.OrderBy(p => p.ProductName);
                    break;
                case "name_desc":
                    productsQuery = productsQuery.OrderByDescending(p => p.ProductName);
                    break;
                case "price_asc":
                    productsQuery = productsQuery.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    productsQuery = productsQuery.OrderByDescending(p => p.Price);
                    break;
                default:
                    productsQuery = productsQuery.OrderBy(p => p.Id); // Default sort
                    break;
            }
            var totalProducts = await productsQuery.CountAsync();
            var products = await productsQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

            var viewModel = new ShopViewModel
            {
                Products = products,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize)
            };

            return View(viewModel);
            //var viewModel = new ProductIndexViewModel
            //{
            //    Products = _context.Products
            //    .Include(p => p.Category)
            //    //.Take(12)
            //    .ToList(),
            //    Categories = _context.Categories.ToList()
            //};

            //return View(viewModel);
        }
        public IActionResult Contact()
        {
            return View();
        }


        // select product 
        public IActionResult Shop_Single(int id)
        {
            var product = _context.Products
                               .Include(p => p.Category) // Include category if needed
                               .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            var viewModel = new ProductIndexVm
            {
                Product = product,
                Quantity = 1,
                ErrorMessage = null
            };

            return View(viewModel);
        }




        [HttpGet]
        public IActionResult CheckOut()
        {
            if(!User.Identity.IsAuthenticated)
    {
                return RedirectToAction("Login", "Login");
            }

            var cart = HttpContext.Session.GetObject<List<CartItemVm>>("Cart") ?? new List<CartItemVm>();

            var model = new CheckOutVm
            {
                CartItems = cart
                
            };
            return View(model); 
        }


        //get cart  

        [HttpGet]
        public IActionResult Cart()
        {
            // Get cart from session
            var cart = HttpContext.Session.GetObject<List<CartItemVm>>("Cart") ?? new List<CartItemVm>();
            return View(cart);
        }


        /// Add item to cart
        [HttpPost]
        public IActionResult Cart(int productId, int quantity)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return NotFound();
            }

            // Get cart from session
            var cart = HttpContext.Session.GetObject<List<CartItemVm>>("Cart") ?? new List<CartItemVm>();

            // Check if item already exists
            var existingItem = cart.FirstOrDefault(c => c.Product.Id == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.TotalPrice = existingItem.Quantity * existingItem.Product.Price ??0;
            }
            else
            {
                cart.Add(new CartItemVm
                {
                    Product = product,
                    Quantity = quantity,
                    TotalPrice = quantity * (product.Price ?? 0)

                });
            }

            // Save cart to session
            HttpContext.Session.SetObject("Cart", cart);

            return View(cart);
        }

        ///  Remove item from cart

        [HttpGet]
        public IActionResult RemoveFromCart(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItemVm>>("Cart") ?? new List<CartItemVm>();

            var itemToRemove = cart.FirstOrDefault(c => c.Product.Id == id);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                HttpContext.Session.SetObject("Cart", cart);
            }

            return RedirectToAction("Cart");
        }

        public IActionResult ThankYou()
        {
            return View();
        }
        public IActionResult TrackProduct()
        {

            var loggedIO = User.FindFirst("Id")?.Value;
            var orderData = (from o in _context.Orders.Where(x => x.CreatedBy.Equals(loggedIO))
                             join r in _context.Riders
                               on o.RiderId equals r.Id into riderJoin
                             from rider in riderJoin.DefaultIfEmpty()
                            
                             select new
                             {
                                 o.Id,
                                 o.OrderDate,
                                 o.DeliveryStatus,
                                 o.Address,
                                 o.City,
                                 o.ZipCode,
                                 RiderName = rider != null ? rider.Name : "Not Assigned",
                                 RiderPhone = rider != null ? rider.Phone : string.Empty
                             }).ToList(); // <- Materialize to memory

            var orders = orderData.Select(o => new OrderStatusVm
            {
                OrderId = o.Id,
                OrderDate = o.OrderDate,
                DeliveryStatus = o.DeliveryStatus.ToString(),
                Address = o.Address,
                City = o.City,
                ZipCode = o.ZipCode,
                RiderName = o.RiderName,
                RiderPhone = o.RiderPhone,
                Progress = o.DeliveryStatus switch
                {
                    DeliveryStatus.Pending => 10,
                    DeliveryStatus.Assigned => 50,
                    DeliveryStatus.Delivered => 100,
                    
                    _ => 0
                }
            }).ToList();

            return View(orders);
        }

        //[HttpPost]
        //public IActionResult AddToCart(int productId)
        //{
        //    var cart = HttpContext.Session.GetObject<List<int>>("Cart") ?? new List<int>();

        //    cart.Add(productId); // You can replace this logic with actual cart item objects if needed

        //    HttpContext.Session.SetObject("Cart", cart);

        //    return Json(new { count = cart.Count });
        //}

    }
}
