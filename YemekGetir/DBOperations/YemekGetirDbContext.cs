// using YemekGetir.Entities;
using Microsoft.EntityFrameworkCore;

namespace YemekGetir.DBOperations
{
  public class YemekGetirDbContext : DbContext, IYemekGetirDbContext
  {
    public YemekGetirDbContext(DbContextOptions<YemekGetirDbContext> options) : base(options)
    { }
    public override int SaveChanges()
    {
      return base.SaveChanges();
    }
  }
}