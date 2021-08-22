using System;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using YemekGetir.DBOperations;
using YemekGetir.Entities;
using TestSetup;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using YemekGetir.Application.RestaurantOperations.Queries.GetRestaurantById;

namespace Application.RestaurantOperations.Queries.GetRestaurantById
{
  public class GetRestaurantByIdQueryTests : IClassFixture<CommonTestFixture>
  {
    private readonly YemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetRestaurantByIdQueryTests(CommonTestFixture testFixture)
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

      GetRestaurantByIdQuery query = new GetRestaurantByIdQuery(_dbContext, _mapper, mockHttpContextAccessor.Object);
      query.Id = "999";

      FluentActions
        .Invoking(() => query.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Restoran bulunamadı.");
    }

    [Fact]
    public void WhenGivenRestaurantIdIsNotTheSameWithTheRestaurantIdInToken_Handle_ThrowsInvalidOperationException()
    {
      Restaurant restaurant = new Restaurant()
      {
        Name = "restname4654684846546",
        Email = "restnam212214545@example.com",
        Password = "restnam212214545",
        CategoryId = 1
      };

      _dbContext.Restaurants.Add(restaurant);
      _dbContext.SaveChanges();
      Restaurant newRestaurant = _dbContext.Restaurants.SingleOrDefault(r => r.Email == restaurant.Email);


      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", "1")
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      GetRestaurantByIdQuery addAddressCommand = new GetRestaurantByIdQuery(_dbContext, _mapper, mockHttpContextAccessor.Object);
      addAddressCommand.Id = newRestaurant.Id.ToString();

      FluentActions
        .Invoking(() => addAddressCommand.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Yalnızca kendi hesap bilgilerinizi görüntüleyebilirsiniz.");
    }

    [Fact]
    public void WhenValidInputsAreGiven_Address_ShouldBeAddedToRestaurant()
    {
      Restaurant restaurant = new Restaurant()
      {
        Name = "restname465114684846546",
        Email = "res12tnam212214545@example.com",
        Password = "restnam212214545",
        CategoryId = 1
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

      GetRestaurantByIdQuery query = new GetRestaurantByIdQuery(_dbContext, _mapper, mockHttpContextAccessor.Object);
      query.Id = testRestaurant.Id.ToString();

      GetRestaurantByIdViewModel foundRestaurant = query.Handle();

      foundRestaurant.Should().NotBeNull();
    }
  }
}