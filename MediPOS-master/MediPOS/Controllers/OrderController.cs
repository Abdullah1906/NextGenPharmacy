using MediPOS.DB;
using MediPOS.Help;
using MediPOS.Hubs;
using MediPOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MediPOS.Controllers
{
    public class OrderController : Controller
    {
        private readonly DataContext _context;
        private readonly IHubContext<OrderHub> _hubContext;
        public OrderController(DataContext context, IHubContext<OrderHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(CheckOutVm model)
        {
            var riders = _context.Riders
            .Where(r => r.IsAvailable)
            .Select(r => new Rider
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();
            //  Get Cart from Session
            var cart = HttpContext.Session.GetObject<List<CartItemVm>>("Cart");
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("CheckOut"); 
            }

            //  Calculate totals
            decimal subtotal = cart.Sum(item => (item.Product.Price ?? 0) * item.Quantity);
            decimal vat = subtotal * 0.05m;
            int total = (int)Math.Round(subtotal + vat);

            //  Create Order object 
            var order = new Order
            {
                CustomerName = model.OrderInfo.FirstName + " " + model.OrderInfo.LastName,
                Address = model.OrderInfo.Address,
                City = model.OrderInfo.City,
                ZipCode = model.OrderInfo.Zip,
                Email = model.OrderInfo.Email,
                Phone = model.OrderInfo.Phone,
                OrderDate = DateTime.Now,
                SubTotal = subtotal,
                TotalAmount = total,
                CreatedAt = DateTime.Now,
                CreatedBy = User.FindFirst("Id")?.Value,
            };

            //  Add order items
            order.OrderItems = cart.Select(item => new OrderItem
            {
                ProductId = item.Product.Id,
                Quantity = item.Quantity,
                UnitPrice = item.Product.Price ?? 0,
                TotalPrice = (item.Product.Price ?? 0) * item.Quantity
            }).ToList();

            
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var fullOrder = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == order.Id);
            var adminConnection = (from a in _context.Users
                                   join b in _context.HubConnectionManages on a.Id equals b.UserId
                                   where b.IsConnectionActive == true && a.userTypeId == 1
                                   select new HubConnectionManage
                                   {
                                       ConnectionId = b.ConnectionId
                                   }).ToList();

            await _hubContext.Clients.Clients(adminConnection.Select(x=>x.ConnectionId).ToList()).SendAsync("NewOrderPlaced", new
            {
                orderId = fullOrder.Id,
                customerName = fullOrder.CustomerName,
                totalAmount = fullOrder.TotalAmount,
                orderDate = fullOrder.OrderDate,
                orderItems = fullOrder.OrderItems.Select(i => new {
                    productName = i.Product?.ProductName,
                    unitPrice = i.UnitPrice,
                    quantity = i.Quantity
                }),
                availableRiders = riders.Select(r => new { id = r.Id, name = r.Name })
            });


            return RedirectToAction("ThankYou", "Landing");
        }

    }
}
