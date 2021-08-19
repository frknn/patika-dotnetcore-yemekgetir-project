using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using YemekGetir.DBOperations;
using YemekGetir.Entities;

namespace YemekGetir.Application.RestaurantOperations.Commands.AddAddress
{
  public class AddAddressCommand
  {
    public string Id { get; set; }
    public AddAddressToRestaurantModel Model { get; set; }
    private readonly IYemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddAddressCommand(IYemekGetirDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
      _dbContext = dbContext;
      _mapper = mapper;
      _httpContextAccessor = httpContextAccessor;
    }

    public void Handle()
    {
      Restaurant restaurant = _dbContext.Restaurants.Include(restaurant => restaurant.Address).SingleOrDefault(u => u.Id.ToString() == Id);
      if (restaurant is null)
      {
        throw new InvalidOperationException("Restoran bulunamadı.");
      }

      string requestOwnerId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "tokenHolderId").Value;
      if (requestOwnerId != Convert.ToString(restaurant.Id))
      {
        throw new InvalidOperationException("Yalnızca kendi hesabınıza adres bilgisi ekleyebilirsiniz.");
      }

      if (restaurant.Address is not null)
      {
        throw new InvalidOperationException("Adres bilgisi zaten eklenmiş.");
      }

      Address newAddress = _mapper.Map<Address>(Model);
      restaurant.Address = newAddress;
      _dbContext.SaveChanges();
    }
  }

  public class AddAddressToRestaurantModel
  {
    private string country;
    public string Country
    {
      get { return country; }
      set { country = value.Trim(); }
    }

    private string district;

    public string District
    {
      get { return district; }
      set { district = value.Trim(); }
    }

    private string city;

    public string City
    {
      get { return city; }
      set { city = value.Trim(); }
    }

    private string line1;

    public string Line1
    {
      get { return line1; }
      set { line1 = value.Trim(); }
    }

    private string line2;

    public string Line2
    {
      get { return line2; }
      set { line2 = value.Trim(); }
    }
  }
}