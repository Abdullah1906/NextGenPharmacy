using System.ComponentModel.DataAnnotations;

namespace MediPOS.Models
{
    public class Blog : Base
    {
        [Required]
        [StringLength(200)]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }

        public string? Image { get; set; }

        [StringLength(100)]
        public string? Author { get; set; }

        public DateTime PublishDate { get; set; } = DateTime.Now;

        public string? Tags { get; set; }

        public int ViewCount { get; set; } = 0;

        public bool IsPublished { get; set; } = true;
    }

    public class BlogIndexViewModel
    {
        public Blog Blog { get; set; } = new Blog();
        public List<Blog> Blogs { get; set; } = new List<Blog>();
    }
} 