using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediPOS.Models
{
    public class User: Base
    {
        
        public string? U_NAME { get; set; }
        [Required]
        public string? U_EMAIL { get; set; }
        [Required]
        public string? U_PHONE_NO { get; set; }
        [Required]
        public string? U_PASSWORD { get; set; }
        public int? userTypeId { get; set; }
        [NotMapped]
        [Compare("U_PASSWORD", ErrorMessage = "Passwords do not match.")]
        public string? U_CONFIRM_PASSWORD { get; set; }

    }


}
