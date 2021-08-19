using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using YemekGetir.DBOperations;
using YemekGetir.Entities;

namespace YemekGetir.Application.RestaurantOperations.Commands.DeleteRestaurant
{
  public class DeleteRestaurantCommand
  {
    public string Id { get; set; }
    private readonly IYemekGetirDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteRestaurantCommand(IYemekGetirDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
      _dbContext = dbContext;
      _httpContextAccessor = httpContextAccessor;
    }

    public void Handle()
    {
      Restaurant restaurant = _dbContext.Restaurants.SingleOrDefault(restaurant => Convert.ToString(restaurant.Id) == Id);
      if (restaurant is null)
      {
        throw new InvalidOperationException("Restoran bulunamadı.");
      }

      string requestOwnerId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "tokenHolderId").Value;
      if (requestOwnerId != Convert.ToString(restaurant.Id))
      {
        throw new InvalidOperationException("Yalnızca kendi hesabınızı silebilirsiniz.");
      }

      _dbContext.Restaurants.Remove(restaurant);
      _dbContext.SaveChanges();
    }
  }
}