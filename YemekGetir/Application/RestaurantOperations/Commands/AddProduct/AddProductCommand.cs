using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using YemekGetir.DBOperations;
using YemekGetir.Entities;

namespace YemekGetir.Application.RestaurantOperations.Commands.AddProduct
{
  public class AddProductCommand
  {
    public string Id { get; set; }
    public AddProductModel Model { get; set; }
    private readonly IYemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddProductCommand(IYemekGetirDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
      _dbContext = dbContext;
      _mapper = mapper;
      _httpContextAccessor = httpContextAccessor;
    }

    public void Handle()
    {
      Restaurant restaurant = _dbContext.Restaurants.Include(restaurant => restaurant.Products).SingleOrDefault(restaurant => restaurant.Id.ToString() == Id);
      if (restaurant is null)
      {
        throw new InvalidOperationException("Restoran bulunamadı.");
      }

      string requestOwnerId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "tokenHolderId").Value;
      if (requestOwnerId != Convert.ToString(restaurant.Id))
      {
        throw new InvalidOperationException("Yalnızca kendi restoranınıza ürün ekleyebilirsiniz.");
      }

      bool hasProductWithSameName = restaurant.Products.Any(prod => prod.Name == Model.Name);
      if (hasProductWithSameName)
      {
        throw new InvalidOperationException("Bu isimde bir ürün zaten var.");
      }

      Product product = _mapper.Map<Product>(Model);
      restaurant.Products.Add(product);
      _dbContext.SaveChanges();
    }
  }

  public class AddProductModel
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