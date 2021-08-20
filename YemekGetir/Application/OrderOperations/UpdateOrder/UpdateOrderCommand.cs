using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using YemekGetir.DBOperations;
using YemekGetir.Entities;

namespace YemekGetir.Application.OrderOperations.Commands.UpdateOrder
{
  public class UpdateOrderCommand
  {
    public string Id { get; set; }
    public UpdateOrderModel Model { get; set; }
    private readonly IYemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateOrderCommand(IYemekGetirDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
      _dbContext = dbContext;
      _mapper = mapper;
      _httpContextAccessor = httpContextAccessor;
    }

    public void Handle()
    {
      string requestOwnerId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "tokenHolderId").Value;
      Restaurant restaurant = _dbContext.Restaurants.SingleOrDefault(restaurant => restaurant.Id.ToString() == requestOwnerId);
      Order order = _dbContext.Orders.Include(order => order.Restaurant).SingleOrDefault(order => order.Id.ToString() == Id);
      
      if (order is null)
      {
        throw new InvalidOperationException("Sipariş bulunamadı.");
      }
      if (order.Restaurant.Id != restaurant.Id)
      {
        throw new InvalidOperationException("Sadece kendi restoranınıza ait siparişlerin durumunu güncelleyebilirsiniz.");
      }

      order.StatusId = Model.StatusId;

      _dbContext.SaveChanges();
    }
  }

  public class UpdateOrderModel
  {
    public int StatusId { get; set; }
  }

}