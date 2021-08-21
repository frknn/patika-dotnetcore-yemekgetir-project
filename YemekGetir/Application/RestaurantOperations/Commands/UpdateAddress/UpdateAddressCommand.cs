using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using YemekGetir.DBOperations;
using YemekGetir.Entities;

namespace YemekGetir.Application.RestaurantOperations.Commands.UpdateAddress
{
  public class UpdateAddressCommand
  {
    public string Id { get; set; }
    public UpdateRestaurantAddressModel Model { get; set; }
    private readonly IYemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateAddressCommand(IYemekGetirDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
      _dbContext = dbContext;
      _mapper = mapper;
      _httpContextAccessor = httpContextAccessor;
    }

    public void Handle()
    {
      Restaurant restaurant = _dbContext.Restaurants.Include(restaurant => restaurant.Address).SingleOrDefault(restaurant => restaurant.Id.ToString() == Id);
      if (restaurant is null)
      {
        throw new InvalidOperationException("Restoran bulunamadı.");
      }

      string requestOwnerId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "tokenHolderId").Value;
      if (requestOwnerId != Convert.ToString(restaurant.Id))
      {
        throw new InvalidOperationException("Yalnızca kendi hesabınızın adres bilgisini güncelleyebilirsiniz.");
      }

      if (restaurant.Address is null)
      {
        throw new InvalidOperationException("Adres bilgisi bulunamadı. Önce adres bilgisi ekleyin.");
      }

      Address updatedAddress = _mapper.Map<Address>(Model);

      restaurant.Address.Country = string.Empty == updatedAddress.Country ? restaurant.Address.Country : updatedAddress.Country;
      restaurant.Address.City = string.Empty == updatedAddress.City ? restaurant.Address.City : updatedAddress.City;
      restaurant.Address.District = string.Empty == updatedAddress.District ? restaurant.Address.District : updatedAddress.District;
      restaurant.Address.Line1 = string.Empty == updatedAddress.Line1 ? restaurant.Address.Line1 : updatedAddress.Line1;
      restaurant.Address.Line2 = string.Empty == updatedAddress.Line2 ? restaurant.Address.Line2 : updatedAddress.Line2;

      _dbContext.SaveChanges();
    }
  }

  public class UpdateRestaurantAddressModel
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