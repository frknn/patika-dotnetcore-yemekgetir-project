using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using YemekGetir.DBOperations;
using YemekGetir.Entities;

namespace YemekGetir.Application.CartOperations.Commands.AddProduct
{
  public class AddProductCommand
  {
    public string Id { get; set; }
    public AddProductToCartModel Model { get; set; }
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
      Cart cart = _dbContext.Carts.Include(cart => cart.LineItems).ThenInclude(lineItem => lineItem.Product).SingleOrDefault(cart => cart.Id.ToString() == Id);
      if (cart is null)
      {
        throw new InvalidOperationException("Sepet bulunamadı.");
      }

      string requestOwnerId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "tokenHolderId").Value;
      if (requestOwnerId != cart.UserId.ToString())
      {
        throw new InvalidOperationException("Yalnızca kendi sepetinize ürün ekleyebilirsiniz.");
      }

      Product product = _dbContext.Products.SingleOrDefault(product => product.Id == Model.ProductId);
      if (product is null)
      {
        throw new InvalidOperationException("Ürün bulunamadı.");
      }

      // sepette başka restoranın ürünü varsa ekleme
      bool hasAnyItemFromDiffrentRestaurant = cart.LineItems.Any(item => item.Product.RestaurantId != product.RestaurantId);
      if (hasAnyItemFromDiffrentRestaurant)
      {
        throw new InvalidOperationException("Sepetinizde başka restorandan ürün bulunmaktadır. Bu restorandan ürün ekleyemezsiniz.");
      }

      bool hasSameItemInCart = cart.LineItems.Any(lineItem => lineItem.ProductId == Model.ProductId);
      if (hasSameItemInCart)
      {
        var foundLineItem = cart.LineItems.SingleOrDefault(lineItem => lineItem.ProductId == Model.ProductId);
        foundLineItem.Quantity += Model.Quantity;
        _dbContext.SaveChanges();
        return;
      }

      LineItem lineItem = new LineItem() { Name = product.Name, Quantity = Model.Quantity, Price = product.Price, ProductId = Model.ProductId };

      cart.LineItems.Add(lineItem);
      _dbContext.SaveChanges();
    }
  }

  public class AddProductToCartModel
  {
    public int ProductId { get; set; }
    public int Quantity { get; set; }
  }
}