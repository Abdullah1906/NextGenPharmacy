using MediPOS.Models;
using Microsoft.EntityFrameworkCore;


namespace MediPOS.DB
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
        }
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<SupplierPurchase> SupplierPurchases { get; set; }
        public DbSet<Rider> Riders { get; set; }
        public DbSet<HubConnectionManage> HubConnectionManages { get; set; }
        public DbSet<Blog> Blogs { get; set; }



    }
}
