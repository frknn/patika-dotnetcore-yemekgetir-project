using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using YemekGetir.DBOperations;
using YemekGetir.Entities;

namespace YemekGetir.Application.OrderOperations.Commands.CreateOrder
{
  public class CreateOrderCommand
  {
    private readonly IYemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateOrderCommand(IYemekGetirDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
      _dbContext = dbContext;
      _mapper = mapper;
      _httpContextAccessor = httpContextAccessor;
    }

    public void Handle()
    {
      string requestOwnerId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "tokenHolderId").Value;
      User user = _dbContext.Users
      .Include(user => user.Cart)
        .ThenInclude(cart => cart.LineItems.Where(item => item.isActive))
        .ThenInclude(lineItem => lineItem.Product)
        .ThenInclude(prod => prod.Restaurant)
      .Include(user => user.Orders)
      .Include(user => user.Address)
      .SingleOrDefault(user => user.Id.ToString() == requestOwnerId);

      if (user.Cart.LineItems == null || user.Cart.LineItems.Count == 0)
      {
        throw new InvalidOperationException("Sepetiniz boş. Sipariş oluşturabilmek için sepetinize ürün ekleyiniz.");
      }

      Restaurant restaurant = _dbContext.Restaurants.Include(restaurant => restaurant.Orders).SingleOrDefault(restaurant => restaurant.Id == user.Cart.LineItems.First().Product.Restaurant.Id);
      Console.WriteLine(restaurant.Name);

      Order order = new Order()
      {
        User = user,
        Restaurant = restaurant,
        ShippingAddress = user.Address,
        LineItems = user.Cart.LineItems,
      };


      _dbContext.Orders.Add(order);
      user.Cart.LineItems.ForEach(item => item.isActive = false);

      _dbContext.SaveChanges();
    }
  }

}