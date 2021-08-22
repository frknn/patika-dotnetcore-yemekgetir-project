using System;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using YemekGetir.Application.RestaurantOperations.Commands.AddProduct;
using YemekGetir.DBOperations;
using YemekGetir.Entities;
using TestSetup;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Application.RestaurantOperations.Commands.AddProduct
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
    public void WhenGivenRestaurantIsNotFound_Handle_ThrowsInvalidOperationException()
    {
      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", "1")
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      AddProductCommand command = new AddProductCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.Id = "999";

      FluentActions
        .Invoking(() => command.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Restoran bulunamadı.");
    }

    [Fact]
    public void WhenGivenRestaurantIdIsNotTheSameWithTheRestaurantIdInToken_Handle_ThrowsInvalidOperationException()
    {
      Restaurant restaurant = new Restaurant()
      {
        Name = "restaas4634636346",
        Email = "restaas4634636346@example.com",
        Password = "restaas4634636346346364",
        CategoryId = 1,
      };

      _dbContext.Restaurants.Add(restaurant);
      _dbContext.SaveChanges();
      Restaurant newRestaurant = _dbContext.Restaurants.SingleOrDefault(u => u.Email == restaurant.Email);


      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", "1")
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      AddProductCommand addProductCommand = new AddProductCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      addProductCommand.Id = newRestaurant.Id.ToString();
      addProductCommand.Model = new AddProductModel()
      {
        Name = "New Prod",
        Price = 10
      };

      FluentActions
        .Invoking(() => addProductCommand.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Yalnızca kendi restoranınıza ürün ekleyebilirsiniz.");
    }

    [Fact]
    public void WhenGivenRestaurantAlreadyHasAnAddress_Handle_ThrowsInvalidOperationException()
    {

      Restaurant restaurant = new Restaurant()
      {
        Name = "newrest",
        Email = "newrest9874563212@example.com",
        Password = "newrest9874563212",
        CategoryId = 1,
      };

      Product product = new Product()
      {
        Name = "A New Product For Restaurant",
        Price = 11,
        Restaurant = restaurant
      };

      _dbContext.Restaurants.Add(restaurant);
      _dbContext.Products.Add(product);
      _dbContext.SaveChanges();

      Restaurant testRestaurant = _dbContext.Restaurants.SingleOrDefault(r => r.Email == restaurant.Email);

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", testRestaurant.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      AddProductCommand command = new AddProductCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.Id = testRestaurant.Id.ToString();
      command.Model = new AddProductModel()
      {
        Name = "A New Product For Restaurant",
        Price = 11,
      };

      FluentActions
        .Invoking(() => command.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Bu isimde bir ürün zaten var.");
    }

    [Fact]
    public void WhenValidInputsAreGiven_Product_ShouldBeAddedToRestaurant()
    {
      Restaurant restaurant = new Restaurant()
      {
        Name = "newrest",
        Email = "newrest129874563212@example.com",
        Password = "newrest9874563212",
        CategoryId = 1,
      };
      _dbContext.Restaurants.Add(restaurant);
      _dbContext.SaveChanges();

      Restaurant testRestaurant = _dbContext.Restaurants.SingleOrDefault(r => r.Email == restaurant.Email);

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", testRestaurant.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      AddProductCommand command = new AddProductCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.Id = testRestaurant.Id.ToString();
      command.Model = new AddProductModel()
      {
        Name = "A New Product For The Restaurant",
        Price = 11,
      };

      command.Handle();

      Restaurant userAfterUpdate = _dbContext.Restaurants.Include(restaurant => restaurant.Products).SingleOrDefault(r => r.Email == testRestaurant.Email);
      userAfterUpdate.Products.Count.Should().BeGreaterThan(0);
      userAfterUpdate.Products.First().Name.Should().Be(command.Model.Name);
      userAfterUpdate.Products.First().Price.Should().Be(command.Model.Price);
    }
  }
}