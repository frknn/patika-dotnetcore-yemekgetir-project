using System;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using YemekGetir.Application.OrderOperations.Commands.CreateOrder;
using YemekGetir.DBOperations;
using YemekGetir.Entities;
using TestSetup;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Application.OrderOperations.Commands.CreateOrder
{
  public class CreateOrderCommandTests : IClassFixture<CommonTestFixture>
  {
    private readonly YemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;

    public CreateOrderCommandTests(CommonTestFixture testFixture)
    {
      _dbContext = testFixture.Context;
      _mapper = testFixture.Mapper;
    }

    [Fact]
    public void WhenUsersCartIsEmpty_Handle_ThrowsInvalidOperationException()
    {
      User user = new User()
      {
        FirstName = "alsjhfaslşfasfdfdf12412",
        LastName = "lsadhfşlsajdhfals",
        Email = "xcnbwueytwe4564@eahfalsf.com",
        Password = "dmnsvsdv897987dsfwe",
        PhoneNumber = "5552326598"

      };
      Cart cart = new Cart()
      {
        User = user
      };

      _dbContext.Users.Add(user);
      _dbContext.Carts.Add(cart);
      _dbContext.SaveChanges();

      User testUser = _dbContext.Users.SingleOrDefault(u => u.Email == user.Email);

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", testUser.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      CreateOrderCommand command = new CreateOrderCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);

      FluentActions
        .Invoking(() => command.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Sepetiniz boş. Sipariş oluşturabilmek için sepetinize ürün ekleyiniz.");
    }

    [Fact]
    public void WhenUsersCartHasItems_Order_ShouldBeCreated()
    {
      User user = new User()
      {
        FirstName = "alsjhfaslşfasfdfdf12412",
        LastName = "lsadhfşlsajdhfals",
        Email = "xcsfsfe4564@eahfalsf.com",
        Password = "dmnsvsdv897987dsfwe",
        PhoneNumber = "5552326598"

      };
      Cart cart = new Cart()
      {
        User = user
      };
      Restaurant restaurant = new Restaurant()
      {
        Name = "möncvömxncvömxcvn",
        Email = "ajshfsfsffsalşsf@wieufwoief.com",
        Password = "sdnvksvljsdq785456",
        CategoryId = 1,
      };
      Product product = new Product()
      {
        Name = "xcnvwerwelksdjglksdg",
        Price = 33,
        Restaurant = restaurant,
      };
      LineItem lineItem = new LineItem()
      {
        Name = product.Name,
        isActive = true,
        Price = product.Price,
        Quantity = 5,
        Product = product
      };

      _dbContext.Users.Add(user);
      _dbContext.Carts.Add(cart);
      _dbContext.Restaurants.Add(restaurant);
      _dbContext.Products.Add(product);
      _dbContext.SaveChanges();

      User testUser = _dbContext.Users.Include(user => user.Cart).ThenInclude(cart => cart.LineItems).SingleOrDefault(u => u.Email == user.Email);
      testUser.Cart.LineItems.Add(lineItem);
      _dbContext.SaveChanges();

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", testUser.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      CreateOrderCommand command = new CreateOrderCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);

      FluentActions.Invoking(() => command.Handle()).Invoke();

      _dbContext.Orders.Should().NotBeNullOrEmpty();
      _dbContext.Orders.Count().Should().BeGreaterThan(0);
    }

  }
}