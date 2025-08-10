using MediPOS.DB;
using MediPOS.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MediPOS.Hubs
{
    public class OrderHub:Hub
    {
        private IHubContext<OrderHub> _hub;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OrderHub(IHubContext<OrderHub> hub,DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _hub = hub;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }



        public override Task OnConnectedAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirst("Id")?.Value;

            var newConnection = new HubConnectionManage
            {
                ConnectionId = Context.ConnectionId,
                UserId = Convert.ToInt32(userId)
            };
            _context.HubConnectionManages.Add(newConnection);
            _context.SaveChanges();

            return base.OnConnectedAsync();
        }

      
        public override  Task OnDisconnectedAsync(Exception exception)
        {
           var existConneciton =  _context.HubConnectionManages.Where(x=>x.ConnectionId == Context.ConnectionId).FirstOrDefault();
            if(existConneciton != null)
            {
                existConneciton.IsConnectionActive = false;
                _context.HubConnectionManages.Update(existConneciton);
                _context.SaveChanges();
                
            }
          
            return base.OnDisconnectedAsync(exception);
        }
    }
}
