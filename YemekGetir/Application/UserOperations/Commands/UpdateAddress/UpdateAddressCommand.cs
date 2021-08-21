using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using YemekGetir.DBOperations;
using YemekGetir.Entities;

namespace YemekGetir.Application.UserOperations.Commands.UpdateAddress
{
  public class UpdateAddressCommand
  {
    public string Id { get; set; }
    public UpdateUserAddressModel Model { get; set; }
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
      User user = _dbContext.Users.Include(user => user.Address).SingleOrDefault(user => user.Id.ToString() == Id);
      if (user is null)
      {
        throw new InvalidOperationException("Kullanıcı bulunamadı.");
      }

      string requestOwnerId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "tokenHolderId").Value;
      if (requestOwnerId != Convert.ToString(user.Id))
      {
        throw new InvalidOperationException("Yalnızca kendi hesabınızın adres bilgisini güncelleyebilirsiniz.");
      }

      if (user.Address is null)
      {
        throw new InvalidOperationException("Adres bilgisi bulunamadı. Önce adres bilgisi ekleyin.");
      }

      Address updatedAddress = _mapper.Map<Address>(Model);

      user.Address.Country = string.Empty == updatedAddress.Country ? user.Address.Country : updatedAddress.Country;
      user.Address.City = string.Empty == updatedAddress.City ? user.Address.City : updatedAddress.City;
      user.Address.District = string.Empty == updatedAddress.District ? user.Address.District : updatedAddress.District;
      user.Address.Line1 = string.Empty == updatedAddress.Line1 ? user.Address.Line1 : updatedAddress.Line1;
      user.Address.Line2 = string.Empty == updatedAddress.Line2 ? user.Address.Line2 : updatedAddress.Line2;

      _dbContext.SaveChanges();
    }
  }

  public class UpdateUserAddressModel
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