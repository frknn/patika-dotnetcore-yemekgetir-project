// using YemekGetir.Entities;
using Microsoft.EntityFrameworkCore;

namespace YemekGetir.DBOperations
{
  public interface IYemekGetirDbContext
  {
    int SaveChanges();
  }
}