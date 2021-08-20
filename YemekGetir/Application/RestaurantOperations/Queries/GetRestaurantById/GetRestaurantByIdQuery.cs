using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using YemekGetir.DBOperations;
using YemekGetir.Entities;

namespace YemekGetir.Application.RestaurantOperations.Queries.GetRestaurantById
{
  public class GetRestaurantByIdQuery
  {
    public string Id { get; set; }
    private readonly IYemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;

    private readonly IHttpContextAccessor _httpContextAccessor;


    public GetRestaurantByIdQuery(IYemekGetirDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
      _dbContext = dbContext;
      _mapper = mapper;
      _httpContextAccessor = httpContextAccessor;
    }

    public GetRestaurantByIdViewModel Handle()
    {
      Restaurant restaurant = _dbContext.Restaurants.Where(restaurant => restaurant.Id.ToString() == Id)
        .Include(restaurant => restaurant.Orders).ThenInclude(order => order.LineItems)
        .Include(restaurant => restaurant.Orders).ThenInclude(order => order.User)
        .Include(restaurant => restaurant.Orders).ThenInclude(order => order.ShippingAddress)
        .Include(restaurant => restaurant.Address)
        .Include(restaurant => restaurant.Products)
        .SingleOrDefault();

      if (restaurant is null)
      {
        throw new InvalidOperationException("Restoran bulunamadı.");
      }

      string requestOwnerId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "tokenHolderId").Value;
      if (requestOwnerId != Convert.ToString(restaurant.Id))
      {
        throw new InvalidOperationException("Yalnızca kendi hesap bilgilerinizi görüntüleyebilirsiniz.");
      }

      GetRestaurantByIdViewModel restaurantVM = _mapper.Map<GetRestaurantByIdViewModel>(restaurant);
      return restaurantVM;
    }
  }

  public class GetRestaurantByIdViewModel
  {
    public string Email { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public GetRestaurantByIdAddressVM Address { get; set; }
    public List<GetRestaurantByIdOrderVM> Orders { get; set; }
    public List<GetRestaurantByIdProductVM> Products { get; set; }
  }

  public class GetRestaurantByIdOrderVM
  {
    public GetRestaurantByIdUserVM User { get; set; }
    public Address ShippingAddress { get; set; }
    public string Status { get; set; }
    public int TotalPrice { get; set; }
    public List<GetRestaurantByIdLineItemVM> LineItems { get; set; }
  }

  public class GetRestaurantByIdAddressVM
  {
    public string Country { get; set; }
    public string District { get; set; }
    public string City { get; set; }
    public string Line1 { get; set; }
    public string Line2 { get; set; }
  }

  public class GetRestaurantByIdUserVM
  {
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
  }

  public class GetRestaurantByIdProductVM
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
  }

  public class GetRestaurantByIdLineItemVM
  {
    public string Name { get; set; }
    public int Quantity { get; set; }
    public int Price { get; set; }
    public int TotalPrice { get; set; }
  }

}