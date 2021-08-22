using System;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using YemekGetir.Application.CartOperations.Commands.AddProduct;
using YemekGetir.DBOperations;
using YemekGetir.Entities;
using TestSetup;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Application.CartOperations.Commands.AddProduct
{
  public class AddProductCommandTests : IClassFixture<CommonTestFixture>
  {
    private readonly YemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;

    public AddProductCommandTests(CommonTestFixture testFixture)
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

      AddProductCommand command = new AddProductCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.Id = 999;

      FluentActions
        .Invoking(() => command.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Sepet bulunamadı.");
    }

    [Fact]
    public void WhenGivenProductIdIsNotFound_Handle_ThrowsInvalidOperationException()
    {
      User user = new User()
      {
        FirstName = "User FN to test cart",
        LastName = "User LN to test cart",
        Email = "usermail546456totcart1212@example.com",
        Password = "userpwwww",
        PhoneNumber = "5555555555"
      };

      Cart cart = new Cart()
      {
        User = user
      };

      _dbContext.Users.Add(user);
      _dbContext.Carts.Add(cart);
      _dbContext.SaveChanges();

      User newUser = _dbContext.Users.SingleOrDefault(u => u.Email == user.Email);

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", newUser.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      AddProductCommand addProductCommand = new AddProductCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      addProductCommand.Id = 1;
      addProductCommand.Model = new AddProductToCartModel()
      {
        ProductId = 999,
        Quantity = 1
      };

      FluentActions
        .Invoking(() => addProductCommand.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Ürün bulunamadı.");
    }

    [Fact]
    public void WhenGivenCartIdIsNotTheUsersCartsId_Handle_ThrowsInvalidOperationException()
    {
      User user1 = new User()
      {
        FirstName = "U1111ser FN to test cart",
        LastName = "User LN to test cart",
        Email = "user7878totestcar2152125t@example.com",
        Password = "userpwwww",
        PhoneNumber = "5555555555"
      };

      Cart cart1 = new Cart()
      {
        User = user1
      };
      User user2 = new User()
      {
        FirstName = "Us65667er 2 FN to test cart",
        LastName = "User 2 LN to test cart",
        Email = "usermail2t2525otestcart@example.com",
        Password = "userpwwww",
        PhoneNumber = "5555555555"
      };

      Cart cart2 = new Cart()
      {
        User = user2
      };

      Product prod1 = new Product()
      {
        Name = "Prod To Test Add Prod To Cart69632",
        Price = 50
      };

      _dbContext.Users.AddRange(user1, user2);
      _dbContext.Carts.AddRange(cart1, cart2);
      _dbContext.Products.Add(prod1);
      _dbContext.SaveChanges();

      User newUser = _dbContext.Users.SingleOrDefault(u => u.Email == user1.Email);
      User otherUser = _dbContext.Users.Include(user => user.Cart).SingleOrDefault(u => u.Email == user2.Email);

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", newUser.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      AddProductCommand addProductCommand = new AddProductCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      addProductCommand.Id = otherUser.Cart.Id;
      addProductCommand.Model = new AddProductToCartModel()
      {
        ProductId = 1,
        Quantity = 1
      };

      FluentActions
        .Invoking(() => addProductCommand.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Yalnızca kendi sepetinize ürün ekleyebilirsiniz.");
    }


    [Fact]
    public void WhenGivenCartHasAnItemFromDifferentRestaurant_Handle_ThrowsInvalidOperationException()
    {
      User user = new User()
      {
        FirstName = "User F5555N to test cartasfafasfasfasfasf",
        LastName = "User LN to test cart",
        Email = "us0505il445r2152125t@example.com",
        Password = "userpwwww",
        PhoneNumber = "5555555555"
      };
      Cart cart = new Cart()
      {
        User = user
      };
      Restaurant restaurant1 = new Restaurant()
      {
        Name = "newrest",
        Email = "n19874563212@example.com",
        Password = "newrest9874563212",
        CategoryId = 1,
      };
      Restaurant restaurant2 = new Restaurant()
      {
        Name = "newrest",
        Email = "newres9999t984512@example.com",
        Password = "newrest9874563212",
        CategoryId = 1,
      };

      Product product1 = new Product()
      {
        Name = "A New Product For Cart",
        Price = 11,
        Restaurant = restaurant1
      };
      Product product2 = new Product()
      {
        Name = "A New Product For Ca555r565656t",
        Price = 11,
        Restaurant = restaurant2
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
      _dbContext.Products.AddRange(product1, product2);
      _dbContext.Users.Add(user);
      _dbContext.Restaurants.AddRange(restaurant1, restaurant2);
      _dbContext.SaveChanges();

      Cart testCart = _dbContext.Carts.Include(cart => cart.LineItems).SingleOrDefault(c => c.User.FirstName == user.FirstName);
      User testUser = _dbContext.Users.SingleOrDefault(u => u.FirstName == user.FirstName);
      testCart.LineItems.Add(lineItem);
      _dbContext.SaveChanges();

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", testUser.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      AddProductCommand command = new AddProductCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.Id = testCart.Id;
      command.Model = new AddProductToCartModel()
      {
        ProductId = 2,
        Quantity = 2
      };

      FluentActions
        .Invoking(() => command.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Sepetinizde başka restorandan ürün bulunmaktadır. Bu restorandan ürün ekleyemezsiniz.");
    }

    [Fact]
    public void WhenGivenCartHasSameItem_LineItemsQuantity_ShouldBeIncrementedByGivenQuantity()
    {
      User user = new User()
      {
        FirstName = "Us8888erfasfasfasfasf",
        LastName = "User LN to test cart",
        Email = "u9995t@example.com",
        Password = "userpwwww",
        PhoneNumber = "5555555555"
      };
      Cart cart = new Cart()
      {
        User = user
      };
      Restaurant restaurant1 = new Restaurant()
      {
        Name = "newrest",
        Email = "new5555563212@example.com",
        Password = "newrest9874563212",
        CategoryId = 1,
      };

      Product product1 = new Product()
      {
        Name = "A New Product For Cart545454545",
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
      _dbContext.LineItems.Add(lineItem);

      _dbContext.SaveChanges();

      Cart testCart = _dbContext.Carts.Include(cart => cart.LineItems).SingleOrDefault(c => c.User.FirstName == user.FirstName);
      User testUser = _dbContext.Users.SingleOrDefault(u => u.FirstName == user.FirstName);
      Product testProd = _dbContext.Products.SingleOrDefault(p => p.Name == product1.Name);
      testCart.LineItems.Add(lineItem);
      _dbContext.SaveChanges();

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", testUser.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      AddProductCommand command = new AddProductCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.Id = testCart.Id;
      command.Model = new AddProductToCartModel()
      {
        ProductId = testProd.Id,
        Quantity = 2
      };

      LineItem testLineItem = _dbContext.LineItems.SingleOrDefault(li => li.Name == product1.Name);
      FluentActions.Invoking(() => command.Handle()).Invoke();
      testLineItem.Should().NotBeNull();
      testLineItem.Quantity.Should().Be(7);
    }

    [Fact]
    public void WhenValidInputsAreGiven_Product_ShouldBeAddedToCart()
    {
      User user = new User()
      {
        FirstName = "Userfasfasfasfasf",
        LastName = "User LN to test cart",
        Email = "us92152444125t@example.com",
        Password = "userpwwww",
        PhoneNumber = "5555555555"
      };
      Cart cart = new Cart()
      {
        User = user
      };
      Restaurant restaurant1 = new Restaurant()
      {
        Name = "newrest",
        Email = "newrest9874563212@example.com",
        Password = "newrest9874563212",
        CategoryId = 1,
      };

      Product product1 = new Product()
      {
        Name = "A New Product For Cart",
        Price = 11,
        Restaurant = restaurant1
      };

      _dbContext.Carts.Add(cart);
      _dbContext.Products.Add(product1);
      _dbContext.Users.Add(user);
      _dbContext.Restaurants.Add(restaurant1);

      _dbContext.SaveChanges();

      Cart testCart = _dbContext.Carts.Include(cart => cart.LineItems).SingleOrDefault(c => c.User.FirstName == user.FirstName);
      User testUser = _dbContext.Users.SingleOrDefault(u => u.FirstName == user.FirstName);

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", testUser.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      AddProductCommand command = new AddProductCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.Id = testCart.Id;
      command.Model = new AddProductToCartModel()
      {
        ProductId = 1,
        Quantity = 2
      };

      FluentActions.Invoking(() => command.Handle()).Invoke();
      testCart.LineItems.Should().NotBeNullOrEmpty();
      // testLineItem.Should().NotBeNull();
      // testLineItem.Quantity.Should().Be(7);
    }
  }
}