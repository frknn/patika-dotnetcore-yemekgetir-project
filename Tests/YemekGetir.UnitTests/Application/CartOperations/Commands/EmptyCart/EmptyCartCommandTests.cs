using System;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using YemekGetir.Application.CartOperations.Commands.EmptyCart;
using YemekGetir.DBOperations;
using YemekGetir.Entities;
using TestSetup;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Application.CartOperations.Commands.EmptyCart
{
  public class EmptyCartCommandTests : IClassFixture<CommonTestFixture>
  {
    private readonly YemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;

    public EmptyCartCommandTests(CommonTestFixture testFixture)
    {
      _dbContext = testFixture.Context;
      _mapper = testFixture.Mapper;
    }

    [Fact]
    public void WhenGivenCartIsNotFound_Handle_ThrowsInvalidOperationException()
    {
      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", "1")
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      EmptyCartCommand command = new EmptyCartCommand(_dbContext, mockHttpContextAccessor.Object);
      command.Id = "999";

      FluentActions
        .Invoking(() => command.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Sepet bulunamadı.");
    }


    [Fact]
    public void WhenGivenCartIdIsNotTheUsersCartsId_Handle_ThrowsInvalidOperationException()
    {
      User user1 = new User()
      {
        FirstName = "vcsdgwegsd",
        LastName = "gfhfgrtfghf",
        Email = "bnmbnmddgwr2@example.com",
        Password = "userdfdhdfhrpwwww",
        PhoneNumber = "5555555555"
      };

      Cart cart1 = new Cart()
      {
        User = user1
      };
      User user2 = new User()
      {
        FirstName = "asfastyıtıyrer",
        LastName = "bnmnmbnmbart",
        Email = "us9cvcccc7a9rt@example.com",
        Password = "userpwwww",
        PhoneNumber = "5555555555"
      };

      Cart cart2 = new Cart()
      {
        User = user2
      };

      _dbContext.Users.AddRange(user1, user2);
      _dbContext.Carts.AddRange(cart1, cart2);
      _dbContext.SaveChanges();

      User newUser = _dbContext.Users.Include(user => user.Cart).ThenInclude(cart => cart.LineItems).SingleOrDefault(u => u.Email == user1.Email);
      User otherUser = _dbContext.Users.Include(user => user.Cart).SingleOrDefault(u => u.Email == user2.Email);

      _dbContext.SaveChanges();


      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", newUser.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      EmptyCartCommand emptyCartCommand = new EmptyCartCommand(_dbContext, mockHttpContextAccessor.Object);
      emptyCartCommand.Id = otherUser.Cart.Id.ToString();
 
      FluentActions
        .Invoking(() => emptyCartCommand.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Yalnızca kendi sepetinizdeki ürünleri silebilirsiniz.");
    }

    [Fact]
    public void WhenValidInputsAreGiven_Cart_ShouldBeEmptied()
    {
      User user = new User()
      {
        FirstName = "qwrysafasfasfasqıuwryqwur",
        LastName = "wqrqwryuyu",
        Email = "fdfdfdfdf444125t@example.com",
        Password = "usedgdgdgrpwwww",
        PhoneNumber = "5555555555"
      };
      Cart cart = new Cart()
      {
        User = user
      };
      Restaurant restaurant1 = new Restaurant()
      {
        Name = "netyasfasfasftyyest",
        Email = "newrtyyyhasfasfh63212@example.com",
        Password = "newrhgghgh4563212",
        CategoryId = 1,
      };

      Product product1 = new Product()
      {
        Name = "yutyt8asfasfasfafs7788hjghj",
        Price = 11,
        Restaurant = restaurant1
      };

      LineItem lineItem = new LineItem()
      {
        isActive = true,
        Name = product1.Name,
        Price = product1.Price,
        Product = product1,
        Quantity = 5,
      };

      _dbContext.Carts.Add(cart);
      _dbContext.Products.Add(product1);
      _dbContext.Users.Add(user);
      _dbContext.Restaurants.Add(restaurant1);

      _dbContext.SaveChanges();

      Cart testCart = _dbContext.Carts.Include(cart => cart.LineItems).SingleOrDefault(c => c.User.FirstName == user.FirstName);
      User testUser = _dbContext.Users.Include(user => user.Cart).SingleOrDefault(u => u.FirstName == user.FirstName);

      testUser.Cart.LineItems.Add(lineItem);
      _dbContext.SaveChanges();
      
      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", testUser.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      EmptyCartCommand command = new EmptyCartCommand(_dbContext, mockHttpContextAccessor.Object);
      command.Id = testCart.Id.ToString();

      FluentActions.Invoking(() => command.Handle()).Invoke();

      Cart cartAfterEmptyOperation = _dbContext.Carts.Include(cart => cart.LineItems.Where(li => li.isActive)).AsNoTracking().SingleOrDefault(cart => cart.Id == testCart.Id);
      cartAfterEmptyOperation.LineItems.Count.Should().Be(0);
    }
  }
}
