using System;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using YemekGetir.Application.RestaurantOperations.Commands.UpdateProduct;
using YemekGetir.DBOperations;
using YemekGetir.Entities;
using TestSetup;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Application.RestaurantOperations.Commands.UpdateProduct
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
    public void WhenGivenRestaurantIsNotFound_Handle_ThrowsInvalidOperationException()
    {
      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", "1")
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      UpdateProductCommand command = new UpdateProductCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.RestaurantId = "999";

      FluentActions
        .Invoking(() => command.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Restoran bulunamadı.");
    }

    [Fact]
    public void WhenGivenProductIsNotFound_Handle_ThrowsInvalidOperationException()
    {
      Restaurant restaurant = new Restaurant()
      {
        Name = "REST NAME 46546465",
        Email = "rstname4654999644568787@example.com",
        Password = "rstname4652124568787",
        CategoryId = 1
      };

      Product product = new Product()
      {
        Name = "A New Product Name For Update",
        Price = 10,
        Restaurant = restaurant
      };

      _dbContext.Restaurants.Add(restaurant);
      _dbContext.Products.Add(product);
      _dbContext.SaveChanges();

      Restaurant newRestaurant = _dbContext.Restaurants.Include(restaurant => restaurant.Products).SingleOrDefault(r => r.Email == restaurant.Email);

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", restaurant.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      UpdateProductCommand updateProductCommand = new UpdateProductCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      updateProductCommand.RestaurantId = newRestaurant.Id.ToString();
      updateProductCommand.ProductId = "999";

      FluentActions
        .Invoking(() => updateProductCommand.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Ürün bulunamadı.");
    }

    [Fact]
    public void WhenGivenRestaurantIdIsNotTheSameWithTheRestaurantIdInToken_Handle_ThrowsInvalidOperationException()
    {
      Restaurant restaurant = new Restaurant()
      {
        Name = "AnewRest684686868",
        Email = "restnew96564564454@example.com",
        Password = "rest654646546545",
        CategoryId = 1
      };
      Product product = new Product()
      {
        Name = "A New Product Name For Update Test",
        Price = 10,
        Restaurant = restaurant
      };

      _dbContext.Restaurants.Add(restaurant);
      _dbContext.Products.Add(product);
      _dbContext.SaveChanges();

      Restaurant newRestaurant = _dbContext.Restaurants.SingleOrDefault(r => r.Email == restaurant.Email);
      Product newProduct = _dbContext.Products.SingleOrDefault(p => p.Name == product.Name);

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", "1")
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      UpdateProductCommand updateProductCommand = new UpdateProductCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      updateProductCommand.RestaurantId = newRestaurant.Id.ToString();
      updateProductCommand.ProductId = newProduct.Id.ToString();

      FluentActions
        .Invoking(() => updateProductCommand.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Yalnızca kendi restoranınıza ait ürün bilgisini güncelleyebilirsiniz.");
    }

    [Fact]
    public void WhenValidInputsAreGiven_Product_ShouldBeUpdated()
    {
      Restaurant restaurant = new Restaurant()
      {
        Name = "newrest879798",
        Email = "resttotestupdateprod7798798@example.com",
        Password = "resttotestupdateprod7798798",
        CategoryId = 1
      };

      Product product = new Product()
      {
        Name = "Prod to Test Update",
        Price = 20,
        Restaurant = restaurant
      };

      _dbContext.Restaurants.Add(restaurant);
      _dbContext.Products.Add(product);
      _dbContext.SaveChanges();

      Restaurant testRestaurant = _dbContext.Restaurants.SingleOrDefault(r => r.Email == restaurant.Email);
      Product testProduct = _dbContext.Products.SingleOrDefault(p => p.Name == product.Name);

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", testRestaurant.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      UpdateProductCommand command = new UpdateProductCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.RestaurantId = testRestaurant.Id.ToString();
      command.ProductId = testProduct.Id.ToString();
      var model = new UpdateProductModel()
      {
        Name = "Updated Name",
        Price = 25
      };
      command.Model = model;

      command.Handle();

      Product productAfterUpdate = _dbContext.Products.SingleOrDefault(p => p.Id == testProduct.Id);

      productAfterUpdate.Should().NotBeNull();
      productAfterUpdate.Name.Should().Be(model.Name);
      productAfterUpdate.Price.Should().Be(model.Price);
     
    }
  }
}