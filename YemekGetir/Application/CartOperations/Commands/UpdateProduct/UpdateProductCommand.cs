using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using YemekGetir.DBOperations;
using YemekGetir.Entities;

namespace YemekGetir.Application.CartOperations.Commands.UpdateProduct
{
  public class UpdateProductCommand
  {
    public string CartId { get; set; }
    public string LineItemId { get; set; }
    public UpdateProductInCartModel Model { get; set; }
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
      Cart cart = _dbContext.Carts.Include(cart => cart.LineItems).ThenInclude(lineItem => lineItem.Product).SingleOrDefault(cart => cart.Id.ToString() == CartId);
      if (cart is null)
      {
        throw new InvalidOperationException("Sepet bulunamadı.");
      }

      LineItem lineItem = _dbContext.LineItems.SingleOrDefault(li => li.Id.ToString() == LineItemId);
      if (lineItem is null)
      {
        throw new InvalidOperationException("Ürün bulunamadı.");
      }

      string requestOwnerId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "tokenHolderId").Value;
      if (requestOwnerId != cart.UserId.ToString())
      {
        throw new InvalidOperationException("Yalnızca kendi sepetinizdeki ürünleri güncelleyebilirsiniz.");
      }

      if(lineItem.Quantity + Model.Quantity <= 0){
        _dbContext.LineItems.Remove(lineItem);
        _dbContext.SaveChanges();
        return;
      }
      lineItem.Quantity += Model.Quantity;
      _dbContext.SaveChanges();
    }
  }

  public class UpdateProductInCartModel
  {
    public int Quantity { get; set; }
  }
}