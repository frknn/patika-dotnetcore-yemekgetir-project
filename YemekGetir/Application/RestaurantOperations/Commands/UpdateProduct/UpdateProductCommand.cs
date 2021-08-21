using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using YemekGetir.DBOperations;
using YemekGetir.Entities;

namespace YemekGetir.Application.RestaurantOperations.Commands.UpdateProduct
{
  public class UpdateProductCommand
  {
    public string RestaurantId { get; set; }
    public string ProductId { get; set; }
    public UpdateProductModel Model { get; set; }
    private readonly IYemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateProductCommand(IYemekGetirDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
      _dbContext = dbContext;
      _mapper = mapper;
      _httpContextAccessor = httpContextAccessor;
    }

    public void Handle()
    {
      Restaurant restaurant = _dbContext.Restaurants.Include(restaurant => restaurant.Products).SingleOrDefault(restaurant => restaurant.Id.ToString() == RestaurantId);
      if (restaurant is null)
      {
        throw new InvalidOperationException("Restoran bulunamadı.");
      }

      Product product = restaurant.Products.SingleOrDefault(product => product.Id.ToString() == ProductId);
      if (product is null)
      {
        throw new InvalidOperationException("Ürün bulunamadı.");
      }

      string requestOwnerId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "tokenHolderId").Value;
      if (requestOwnerId != restaurant.Id.ToString())
      {
        throw new InvalidOperationException("Yalnızca kendi restoranınıza ait ürün bilgisini güncelleyebilirsiniz.");
      }

      product.Name = string.IsNullOrEmpty(Model.Name) ? product.Name : Model.Name;
      product.Price = Model.Price == default ? product.Price : Model.Price;

      _dbContext.SaveChanges();
    }
  }

  public class UpdateProductModel
  {
    private string name;
    public string Name
    {
      get { return name; }
      set { name = value.Trim(); }
    }
    public int Price { get; set; }
  }
}