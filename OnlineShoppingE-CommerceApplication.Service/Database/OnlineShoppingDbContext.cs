using Microsoft.EntityFrameworkCore;
using OnlineShoppingE_CommerceApplication.Provider.Entities;

namespace OnlineShoppingE_CommerceApplication.Service.Database
{
    public class OnlineShoppingDbContext : DbContext
    {
        public OnlineShoppingDbContext(DbContextOptions<OnlineShoppingDbContext> context) : base(context)
        {

        }
        public DbSet<User> User { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Colour> Colour { get; set; }
        public DbSet<Size> Size { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<ProductVariant> ProductVariant { get; set; }
        public DbSet<Stock> Stock { get; set; }
        public DbSet<VProduct> VProduct { get; set; }
        public DbSet<Wishlist> Wishlist { get; set; }
        

    }
}
