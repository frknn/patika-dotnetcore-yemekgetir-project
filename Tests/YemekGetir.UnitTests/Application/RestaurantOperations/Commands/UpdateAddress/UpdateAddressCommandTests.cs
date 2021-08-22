using System;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using YemekGetir.Application.RestaurantOperations.Commands.UpdateAddress;
using YemekGetir.DBOperations;
using YemekGetir.Entities;
using TestSetup;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Application.RestaurantOperations.Commands.UpdateAddress
{
  public class UpdateAddressCommandTests : IClassFixture<CommonTestFixture>
  {
    private readonly YemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateAddressCommandTests(CommonTestFixture testFixture)
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

      UpdateAddressCommand command = new UpdateAddressCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
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
        Name = "AnewRest684686868",
        Email = "restnew46846846454@example.com",
        Password = "rest654646546545",
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

      UpdateAddressCommand addAddressCommand = new UpdateAddressCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      addAddressCommand.Id = newRestaurant.Id.ToString();

      FluentActions
        .Invoking(() => addAddressCommand.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Yalnızca kendi hesabınızın adres bilgisini güncelleyebilirsiniz.");
    }

    [Fact]
    public void WhenGivenRestaurantHasNoAddress_Handle_ThrowsInvalidOperationException()
    {
      Restaurant restaurant = new Restaurant()
      {
        Name = "AnewR64565est",
        Email = "res6846454@example.com",
        Password = "rest654646546545",
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

      UpdateAddressCommand command = new UpdateAddressCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.Id = testRestaurant.Id.ToString();
      command.Model = new UpdateRestaurantAddressModel()
      {
        Country = "Türkiye",
        City = "Karabük",
        District = "Merkez",
        Line1 = "Adres satırı 1",
        Line2 = "Adres satırı 2",
      };

      FluentActions
        .Invoking(() => command.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Adres bilgisi bulunamadı. Önce adres bilgisi ekleyin.");
    }

    [Fact]
    public void WhenValidInputsAreGiven_Address_ShouldBeAddedToRestaurant()
    {
      Address address = new Address()
      {
        Country = "Türkiye",
        City = "Karabük",
        District = "Merkez",
        Line1 = "Adres satırı 1",
        Line2 = "Adres satırı 2",
      };

      Restaurant restaurant = new Restaurant()
      {
        Name = "AnewR6465654565est",
        Email = "res87511146454@example.com",
        Password = "rest654646546545",
        CategoryId = 1,
        Address = address
      };

      _dbContext.Restaurants.Add(restaurant);
      _dbContext.Addresses.Add(address);
      _dbContext.SaveChanges();

      Restaurant testRestaurant = _dbContext.Restaurants.Include(restaurant => restaurant.Address).SingleOrDefault(r => r.Email == restaurant.Email);

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", testRestaurant.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      UpdateAddressCommand command = new UpdateAddressCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.Id = testRestaurant.Id.ToString();
      var model = new UpdateRestaurantAddressModel()
      {
        Country = "Updated Country",
        City = "Updated City",
        District = "Updated District",
        Line1 = "Updated Line 1",
        Line2 = "Updated Line 2",
      };
      command.Model = model;

      command.Handle();

      Restaurant userAfterUpdate = _dbContext.Restaurants.Include(user => user.Address).SingleOrDefault(u => u.Email == testRestaurant.Email);
      
      userAfterUpdate.Address.Should().NotBeNull();
      userAfterUpdate.Address.Country.Should().Be(model.Country);
      userAfterUpdate.Address.City.Should().Be(model.City);
      userAfterUpdate.Address.District.Should().Be(model.District);
      userAfterUpdate.Address.Line1.Should().Be(model.Line1);
      userAfterUpdate.Address.Line2.Should().Be(model.Line2);
    }
  }
}