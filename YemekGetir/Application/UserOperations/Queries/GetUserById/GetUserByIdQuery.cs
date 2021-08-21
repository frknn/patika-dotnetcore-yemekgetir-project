using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using YemekGetir.DBOperations;
using YemekGetir.Entities;

namespace YemekGetir.Application.UserOperations.Queries.GetUserById
{
  public class GetUserByIdQuery
  {
    public string Id { get; set; }
    private readonly IYemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;

    private readonly IHttpContextAccessor _httpContextAccessor;


    public GetUserByIdQuery(IYemekGetirDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
      _dbContext = dbContext;
      _mapper = mapper;
      _httpContextAccessor = httpContextAccessor;
    }

    public GetUserByIdViewModel Handle()
    {
      User user = _dbContext.Users.Where(user => user.Id.ToString() == Id)
      .Include(user => user.Cart)
        .ThenInclude(cart => cart.LineItems.Where(item => item.isActive))
        .AsNoTracking()
      .Include(user => user.Orders)
        .ThenInclude(order => order.Restaurant)
      .Include(user => user.Orders)
        .ThenInclude(order => order.LineItems)
      .Include(user => user.Address)
      .SingleOrDefault();

      if (user is null)
      {
        throw new InvalidOperationException("Kullanıcı bulunamadı.");
      }

      string requestOwnerId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "tokenHolderId").Value;
      if (requestOwnerId != Convert.ToString(user.Id))
      {
        throw new InvalidOperationException("Yalnızca kendi hesap bilgilerinizi görüntüleyebilirsiniz.");
      }

      GetUserByIdViewModel userVM = _mapper.Map<GetUserByIdViewModel>(user);
      return userVM;
    }
  }

  public class GetUserByIdViewModel
  {
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public GetUserByIdAddressVM Address { get; set; }
    public GetUserByIdCartVM Cart { get; set; }
    public List<GetUserByIdOrderVM> Orders { get; set; }
  }

  public class GetUserByIdOrderVM
  {
    public string RestaurantName { get; set; }
    public string Status { get; set; }
    public int TotalPrice { get; set; }
    public List<GetUserByIdLineItemVM> LineItems { get; set; }
  }

  public class GetUserByIdCartVM
  {
    public List<GetUserByIdLineItemVM> LineItems { get; set; }
    public int TotalPrice { get; set; }
  }

  public class GetUserByIdAddressVM
  {
    public string Country { get; set; }
    public string District { get; set; }
    public string City { get; set; }
    public string Line1 { get; set; }
    public string Line2 { get; set; }
  }

  public class GetUserByIdLineItemVM
  {
    public string Name { get; set; }
    public int Quantity { get; set; }
    public int Price { get; set; }
    public int TotalPrice { get; set; }
  }

}