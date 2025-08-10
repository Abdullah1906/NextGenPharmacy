using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediPOS.DB;
using MediPOS.Models;

namespace MediPOS.Controllers
{
    public class BlogController : Controller
    {
        private readonly DataContext _context;

        public BlogController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var blogs = await _context.Blogs
                .Where(b => b.IsPublished && !b.IsDelete)
                .OrderByDescending(b => b.PublishDate)
                .ToListAsync();

            return View(blogs);
        }

        public async Task<IActionResult> Details(int id)
        {
            var blog = await _context.Blogs
                .FirstOrDefaultAsync(b => b.Id == id && b.IsPublished && !b.IsDelete);

            if (blog == null)
            {
                return NotFound();
            }

            // Increment view count
            blog.ViewCount++;
            await _context.SaveChangesAsync();

            return View(blog);
        }
    }
} 