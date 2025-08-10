using MediPOS.DB;
using MediPOS.Hubs;
using MediPOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MediPOS.Controllers
{
    public class TrackingController : Controller
    {

        private readonly DataContext _context;
        private readonly IHubContext<DeliveryHub> _hubContext; 
        private readonly IHubContext<AssignHub> _hubContext2;

        public TrackingController(DataContext context, IHubContext<DeliveryHub> hubContext, IHubContext<AssignHub> hubContext2)
        {
            _context = context;
            _hubContext = hubContext;
            _hubContext2 = hubContext2;
            
        }
        public IActionResult Index()
        {
            return View();
        }

        //  assign rider ID 
        [HttpPost]
        public async Task<IActionResult> AssignRider(int orderId, int riderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                var rider = await _context.Riders.FirstOrDefaultAsync(r => r.Id == riderId);

                if (order == null || rider == null)
                    return BadRequest("Invalid order or rider.");

                order.RiderId = rider.Id;
                order.DeliveryStatus = DeliveryStatus.Assigned;
                await _context.SaveChangesAsync();

                // Broadcast to all clients that a rider was assigned
                await _hubContext.Clients.User(rider.Id.ToString()).SendAsync("RiderAssigned", new
                {
                    orderId = order.Id,
                    riderId = rider.Id,
                    riderName = rider.Name,
                    deliveryStatus = order.DeliveryStatus.ToString(),
                    orderItems = order.OrderItems.Select(i => new
                    {
                        productName = i.Product?.ProductName,
                        quantity = i.Quantity,
                        unitPrice = i.UnitPrice
                    })
                });

                return Json(new
                {
                    success = true,
                    orderId = orderId,
                    status = "Assigned",
                    showDeliverButton = true
                });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while assigning the rider.");
            }
        }



        public async Task<IActionResult> Delivered(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                    return NotFound();

                // 1. Update delivery status
                order.DeliveryStatus = DeliveryStatus.Delivered;

                // 2. Save sale record
                var sale = new Sale
                {
                    OrderId = order.Id,
                    Total = order.TotalAmount,
                    SaleDate = order.OrderDate,
                    CreatedAt= DateTime.Now,
                    CreatedBy = User.FindFirst("Id")?.Value
                };
                await _context.Sales.AddAsync(sale);

                // 3. Reduce stock of each product in the order
                foreach (var item in order.OrderItems)
                {
                    var product = item.Product;
                    if (product != null)
                    {
                        product.Stock -= item.Quantity;

                        if (product.Stock < 0)
                            product.Stock = 0; // Avoid negative stock
                    }
                }

                await _context.SaveChangesAsync();

                await _hubContext.Clients.All.SendAsync("OrderDelivered", new
                {
                    orderId = order.Id,
                    customer = order.CustomerName,
                    deliveredAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                });

                return Json(new { success = true, status = "Delivered" });
            }
            catch (Exception ex)
            {
                // You can optionally log the exception here
                return StatusCode(500, "An error occurred while processing the delivery.");
            }
        }



    }
}
