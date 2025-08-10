using System.ComponentModel.DataAnnotations;

namespace MediPOS.Models
{
    public class HubConnectionManage 
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? ConnectionId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsConnectionActive { get; set; } = true;    
    }
}
