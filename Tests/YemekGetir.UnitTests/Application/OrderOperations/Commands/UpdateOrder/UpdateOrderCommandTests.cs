using System;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using YemekGetir.Application.OrderOperations.Commands.UpdateOrder;
using YemekGetir.DBOperations;
using YemekGetir.Entities;
using TestSetup;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using YemekGetir.Common;

namespace Application.OrderOperations.Commands.UpdateOrder
{
  public class UpdateOrderCommandTests : IClassFixture<CommonTestFixture>
  {
    private readonly YemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateOrderCommandTests(CommonTestFixture testFixture)
    {
      _dbContext = testFixture.Context;
      _mapper = testFixture.Mapper;
    }

    [Fact]
    public void WhenGivenOrderIsNotFound_Handle_ThrowsInvalidOperationException()
    {
      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", "1")
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);
      UpdateOrderCommand command = new UpdateOrderCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.Id = 999;

      FluentActions
       .Invoking(() => command.Handle())
       .Should().Throw<InvalidOperationException>()
       .And
       .Message.Should().Be("Sipariş bulunamadı.");
    }

    [Fact]
    public void WhenValidInputsAreGiven_OrderStatusId_ShouldBeUpdated()
    {
      User user = new User()
      {
        FirstName = "ajshfaşlsfhaslşjfalsfdfgwewe",
        LastName = "xcnvmwerwe",
        Email = "qweqwe78979vbcvbs@asnassa.com",
        Password = "cnvxcvnlejwer",
        PhoneNumber = "5456632589"
      };
      Restaurant restaurant = new Restaurant()
      {
        Name = "qowhfqwouhfkjanfafs",
        CategoryId = 1,
        Email = "möxcnvqp8798asljasf@wqfhowfqjwl.com",
        Password = "weyrwerınmsdsdbfafaw",
      };
      Product product = new Product()
      {
        Name = "lashfqowhfqowfqwf",
        Price = 15,
        Restaurant = restaurant
      };
      LineItem lineItem = new LineItem()
      {
        isActive = true,
        Name = product.Name,
        Price = product.Price,
        Quantity = 5,
        Product = product,
      };
      Cart cart = new Cart()
      {
        User = user,
        LineItems = new List<LineItem> { lineItem }
      };

      Order order = new Order()
      {
        User = user,
        Restaurant = restaurant,
        LineItems = cart.LineItems,
      };

      _dbContext.Users.Add(user);
      _dbContext.Restaurants.Add(restaurant);
      _dbContext.Products.Add(product);
      _dbContext.LineItems.Add(lineItem);
      _dbContext.Carts.Add(cart);
      _dbContext.Orders.Add(order);

      _dbContext.SaveChanges();

      Restaurant testRestaurant = _dbContext.Restaurants.Include(restaurant => restaurant.Orders).SingleOrDefault(r => r.Email == restaurant.Email);
      Order testOrder = _dbContext.Orders.Include(order => order.Restaurant).SingleOrDefault(order => order.Restaurant.Id == testRestaurant.Id);

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", testRestaurant.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);
      UpdateOrderCommand command = new UpdateOrderCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.Id = testOrder.Id;
      command.Model = new UpdateOrderModel()
      {
        OrderStatus = (StatusEnum)2
      };

      FluentActions.Invoking(() => command.Handle()).Invoke();
      Order orderAfterUpdate = _dbContext.Orders.SingleOrDefault(order => order.Id == testOrder.Id);
      orderAfterUpdate.StatusId.Should().Be(2);

    }

    [Fact]
    public void WhenGivenOrderIsNotTheRestaurantsOrder_Handle_ThrowsInvalidOperationException()
    {
      User user1 = new User()
      {
        FirstName = "ajshfaşlsfhaslşjfalsfdfgwewe",
        LastName = "xcnvmwerwe",
        Email = "qweqwe78979vbcvbs@asnassa.com",
        Password = "cnvxcvnlejwer",
        PhoneNumber = "5456632589"
      };
      User user2 = new User()
      {
        FirstName = "ajshqwwqwwqfhaslşjfalsfdfgwewe",
        LastName = "xcqqqqqmwerwe",
        Email = "qweqqqqqqqvbs@asnassa.com",
        Password = "cnvxqqqjwer",
        PhoneNumber = "5456632589"
      };
      Restaurant restaurant1 = new Restaurant()
      {
        Name = "qowhfqwousfsfhfkjanfafs",
        CategoryId = 1,
        Email = "möxcnvqfsfp8798asljasf@wqfhowfqjwl.com",
        Password = "weyrwerınmsdsdbfafaw",
      };
      Restaurant restaurant2 = new Restaurant()
      {
        Name = "qowhfqwasfaasfsfsfafsouhfkjanfafs",
        CategoryId = 1,
        Email = "möxcnvsfsfsfasfasfqp8798asljasf@wqfhowfqjwl.com",
        Password = "weyrwerınmsdsdbfafaw",
      };
      Product product1 = new Product()
      {
        Name = "lashfqasfasfasfowhfqowfqwf",
        Price = 15,
        Restaurant = restaurant1
      };
      Product product2 = new Product()
      {
        Name = "lashfdfgvbvbvbqowhfqowfqwf",
        Price = 15,
        Restaurant = restaurant2
      };
      LineItem lineItem1 = new LineItem()
      {
        isActive = true,
        Name = product1.Name,
        Price = product1.Price,
        Quantity = 5,
        Product = product1,
      };
      LineItem lineItem2 = new LineItem()
      {
        isActive = true,
        Name = product2.Name,
        Price = product2.Price,
        Quantity = 5,
        Product = product2,
      };
      Cart cart1 = new Cart()
      {
        User = user1,
        LineItems = new List<LineItem> { lineItem1 }
      };
      Cart cart2 = new Cart()
      {
        User = user2,
        LineItems = new List<LineItem> { lineItem2 }
      };

      Order order1 = new Order()
      {
        User = user1,
        Restaurant = restaurant1,
        LineItems = cart1.LineItems,
      };
      Order order2 = new Order()
      {
        User = user2,
        Restaurant = restaurant2,
        LineItems = cart2.LineItems,
      };

      _dbContext.Users.AddRange(user1, user2);
      _dbContext.Restaurants.AddRange(restaurant1, restaurant2);
      _dbContext.Products.AddRange(product1, product2);
      _dbContext.LineItems.AddRange(lineItem1, lineItem2);
      _dbContext.Carts.AddRange(cart1, cart2);
      _dbContext.Orders.AddRange(order1, order2);

      _dbContext.SaveChanges();

      Restaurant testRestaurant1 = _dbContext.Restaurants.Include(restaurant => restaurant.Orders).SingleOrDefault(r => r.Email == restaurant1.Email);
      Order testOrder1 = _dbContext.Orders.Include(order => order.Restaurant).SingleOrDefault(order => order.Restaurant.Id == testRestaurant1.Id);
      Restaurant testRestaurant2 = _dbContext.Restaurants.Include(restaurant => restaurant.Orders).SingleOrDefault(r => r.Email == restaurant2.Email);
      Order testOrder2 = _dbContext.Orders.Include(order => order.Restaurant).SingleOrDefault(order => order.Restaurant.Id == testRestaurant2.Id);

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", testRestaurant1.Id.ToString())
      };

      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);
      UpdateOrderCommand command = new UpdateOrderCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.Id = testOrder2.Id;
      command.Model = new UpdateOrderModel()
      {
        OrderStatus = (StatusEnum)2
      };

      FluentActions
        .Invoking(() => command.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Sadece kendi restoranınıza ait siparişlerin durumunu güncelleyebilirsiniz.");

    }
  }
}