using YemekGetir.Entities;
using Microsoft.EntityFrameworkCore;

namespace YemekGetir.DBOperations
{
  public class YemekGetirDbContext : DbContext, IYemekGetirDbContext
  {
    public YemekGetirDbContext(DbContextOptions<YemekGetirDbContext> options) : base(options)
    { }
    public DbSet<User> Users { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<LineItem> LineItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public override int SaveChanges()
    {
      return base.SaveChanges();
    }
  }
}