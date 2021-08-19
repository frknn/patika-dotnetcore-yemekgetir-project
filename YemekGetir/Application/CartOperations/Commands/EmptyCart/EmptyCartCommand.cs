using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using YemekGetir.DBOperations;
using YemekGetir.Entities;

namespace YemekGetir.Application.CartOperations.Commands.EmptyCart
{
  public class EmptyCartCommand
  {
    public string Id { get; set; }
    private readonly IYemekGetirDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EmptyCartCommand(IYemekGetirDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
      _dbContext = dbContext;
      _httpContextAccessor = httpContextAccessor;
    }

    public void Handle()
    {
      Cart cart = _dbContext.Carts.Include(cart => cart.LineItems).ThenInclude(lineItem => lineItem.Product).SingleOrDefault(cart => cart.Id.ToString() == Id);
      if (cart is null)
      {
        throw new InvalidOperationException("Sepet bulunamadı.");
      }

      string requestOwnerId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "tokenHolderId").Value;
      if (requestOwnerId != cart.UserId.ToString())
      {
        throw new InvalidOperationException("Yalnızca kendi sepetinizdeki ürünleri silebilirsiniz.");
      }

      cart.LineItems.Clear();
      _dbContext.SaveChanges();
    }
  }
}