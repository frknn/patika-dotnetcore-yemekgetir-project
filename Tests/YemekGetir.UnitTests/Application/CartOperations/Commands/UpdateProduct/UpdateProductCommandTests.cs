using System;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using YemekGetir.Application.CartOperations.Commands.UpdateProduct;
using YemekGetir.DBOperations;
using YemekGetir.Entities;
using TestSetup;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Application.CartOperations.Commands.UpdateProduct
{
  public class UpdateProductCommandTests : IClassFixture<CommonTestFixture>
  {
    private readonly YemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateProductCommandTests(CommonTestFixture testFixture)
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

      UpdateProductCommand command = new UpdateProductCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.CartId = "999";

      FluentActions
        .Invoking(() => command.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Sepet bulunamadı.");
    }

    [Fact]
    public void WhenGivenLineItemIsNotFound_Handle_ThrowsInvalidOperationException()
    {
      User user = new User() { FirstName = "uptyser", LastName = " askjhfakjsfhasf", Email = "aksfalsfhasjflasf@ajkshfasf.com", Password = "asjfakjfhqkfw16546", PhoneNumber = "6668745895" };
      Cart cart = new Cart()
      {
        User = user
      };
      _dbContext.Carts.Add(cart);
      _dbContext.Users.Add(user);
      _dbContext.SaveChanges();

      Cart addedCart = _dbContext.Carts.Include(cart => cart.User).SingleOrDefault(c => c.User.FirstName == cart.User.FirstName);

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", "1")
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      UpdateProductCommand command = new UpdateProductCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.CartId = addedCart.Id.ToString();
      command.LineItemId = "999";

      FluentActions
        .Invoking(() => command.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Ürün bulunamadı.");
    }

    [Fact]
    public void WhenGivenCartIdIsNotTheUsersCartsId_Handle_ThrowsInvalidOperationException()
    {
      User user1 = new User()
      {
        FirstName = "U1879781ser Fst cart",
        LastName = "User LN to test cart",
        Email = "us50509er782152125t@example.com",
        Password = "userpwwww",
        PhoneNumber = "5555555555"
      };

      Cart cart1 = new Cart()
      {
        User = user1
      };
      User user2 = new User()
      {
        FirstName = "U1s999cart",
        LastName = "User 2 LN to test cart",
        Email = "us9087a9rt@example.com",
        Password = "userpwwww",
        PhoneNumber = "5555555555"
      };

      Cart cart2 = new Cart()
      {
        User = user2
      };

      Product prod1 = new Product()
      {
        Name = "Prd Prod To C78787a8888rt69632",
        Price = 50
      };

      LineItem lineItem = new LineItem()
      {
        isActive = true,
        Name = prod1.Name,
        Price = prod1.Price,
        Product = prod1,
        Quantity = 5,
      };

      _dbContext.Users.AddRange(user1, user2);
      _dbContext.Carts.AddRange(cart1, cart2);
      _dbContext.Products.Add(prod1);
      _dbContext.SaveChanges();

      User newUser = _dbContext.Users.Include(user => user.Cart).ThenInclude(cart => cart.LineItems).SingleOrDefault(u => u.Email == user1.Email);
      User otherUser = _dbContext.Users.Include(user => user.Cart).SingleOrDefault(u => u.Email == user2.Email);

      newUser.Cart.LineItems.Add(lineItem);
      _dbContext.SaveChanges();

      // LineItem newLineItem = _dbContext.LineItems.SingleOrDefault(lineItem => lineItem.Name == prod1.Name);

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", newUser.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      UpdateProductCommand updateProductCommand = new UpdateProductCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      updateProductCommand.CartId = otherUser.Cart.Id.ToString();
      updateProductCommand.LineItemId = lineItem.Id.ToString();
      updateProductCommand.Model = new UpdateProductInCartModel()
      {
        Quantity = 1
      };

      FluentActions
        .Invoking(() => updateProductCommand.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Yalnızca kendi sepetinizdeki ürünleri güncelleyebilirsiniz.");
    }

    [Fact]
    public void WhenGivenQuantityIsGreaterThanLineItemsQuantity_LineItem_ShouldBeRemoved()
    {
      User user1 = new User()
      {
        FirstName = "cvbxmbxcv",
        LastName = "xcvbxcvbvb",
        Email = "ucvbcvreywerweyr2125t@example.com",
        Password = "userpwwww",
        PhoneNumber = "5555555555"
      };

      Cart cart1 = new Cart()
      {
        User = user1
      };

      Product prod1 = new Product()
      {
        Name = "pdsjh656sdkjghsdjksdg",
        Price = 2
      };

      LineItem lineItem = new LineItem()
      {
        isActive = true,
        Name = prod1.Name,
        Price = prod1.Price,
        Product = prod1,
        Quantity = 5,
      };

      _dbContext.Users.AddRange(user1);
      _dbContext.Carts.AddRange(cart1);
      _dbContext.Products.Add(prod1);
      _dbContext.SaveChanges();

      User newUser = _dbContext.Users.Include(user => user.Cart).ThenInclude(cart => cart.LineItems).SingleOrDefault(u => u.Email == user1.Email);

      newUser.Cart.LineItems.Add(lineItem);
      _dbContext.SaveChanges();

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", newUser.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      LineItem newLineItem = _dbContext.LineItems.SingleOrDefault(lineItem => lineItem.Name == prod1.Name);

      UpdateProductCommand updateProductCommand = new UpdateProductCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      updateProductCommand.CartId = newUser.Cart.Id.ToString();
      updateProductCommand.LineItemId = newLineItem.Id.ToString();
      updateProductCommand.Model = new UpdateProductInCartModel()
      {
        Quantity = -20
      };

      FluentActions.Invoking(() => updateProductCommand.Handle()).Invoke();

      LineItem testLineItem = _dbContext.LineItems.SingleOrDefault(lineItem => lineItem.Name == prod1.Name);
      testLineItem.Should().BeNull();
    }


    [Fact]
    public void WhenValidInputsAreGiven_Product_ShouldBeAddedToCart()
    {
      User user = new User()
      {
        FirstName = "qwryqıuwryqwur",
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
        Name = "netytyyest",
        Email = "newrtyyyhh63212@example.com",
        Password = "newrhgghgh4563212",
        CategoryId = 1,
      };

      Product product1 = new Product()
      {
        Name = "yutyt87788hjghj",
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

      LineItem testLineItem = _dbContext.LineItems.SingleOrDefault(li => li.Name == product1.Name);
      
      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", testUser.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      UpdateProductCommand command = new UpdateProductCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.CartId = testCart.Id.ToString();
      command.LineItemId = testLineItem.Id.ToString();
      command.Model = new UpdateProductInCartModel()
      {
        Quantity = 2
      };

      FluentActions.Invoking(() => command.Handle()).Invoke();
      LineItem afterUpdateLineItem = _dbContext.LineItems.SingleOrDefault(li => li.Name == product1.Name);

      afterUpdateLineItem.Should().NotBeNull();
      afterUpdateLineItem.Quantity.Should().Be(7);
    }
  }
}
