using System;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using YemekGetir.Application.RestaurantOperations.Commands.AddAddress;
using YemekGetir.DBOperations;
using YemekGetir.Entities;
using TestSetup;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;

namespace Application.RestaurantOperations.Commands.AddAddress
{
  public class AddAddressCommandTests : IClassFixture<CommonTestFixture>
  {
    private readonly YemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;

    public AddAddressCommandTests(CommonTestFixture testFixture)
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

      AddAddressCommand command = new AddAddressCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
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
        Name = "RestName54564686844",
        Email = "rest4568486486465465@example.com",
        Password = "asfkhaslfkasjfas",
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

      AddAddressCommand addAddressCommand = new AddAddressCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      addAddressCommand.Id = newRestaurant.Id.ToString();

      FluentActions
        .Invoking(() => addAddressCommand.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Yalnızca kendi hesabınıza adres bilgisi ekleyebilirsiniz.");
    }

    [Fact]
    public void WhenGivenRestaurantAlreadyHasAnAddress_Handle_ThrowsInvalidOperationException()
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
        Name = "RestName54564686844",
        Email = "rest464656126465465@example.com",
        Password = "asfkhaslfkasjfas",
        CategoryId = 1,
        Address = address
      };
      _dbContext.Restaurants.Add(restaurant);
      _dbContext.Addresses.Add(address);
      _dbContext.SaveChanges();

      Restaurant testRestaurant = _dbContext.Restaurants.SingleOrDefault(r => r.Email == restaurant.Email);

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", testRestaurant.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      AddAddressCommand command = new AddAddressCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.Id = testRestaurant.Id.ToString();
      command.Model = new AddAddressToRestaurantModel()
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
        .Message.Should().Be("Adres bilgisi zaten eklenmiş.");
    }

    [Fact]
    public void WhenValidInputsAreGiven_Address_ShouldBeAddedToRestaurant()
    {
      Restaurant user = new Restaurant()
      {
        Name = "RestName54564686844",
        Email = "rest7897856126465465@example.com",
        Password = "asfkhaslfkasjfas",
        CategoryId = 1,
      };
      _dbContext.Restaurants.Add(user);
      _dbContext.SaveChanges();

      Restaurant testRestaurant = _dbContext.Restaurants.SingleOrDefault(u => u.Email == user.Email);

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", testRestaurant.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      AddAddressCommand command = new AddAddressCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.Id = testRestaurant.Id.ToString();
      command.Model = new AddAddressToRestaurantModel()
      {
        Country = "Türkiye",
        City = "Karabük",
        District = "Merkez",
        Line1 = "Adres satırı 1",
        Line2 = "Adres satırı 2",
      };

      command.Handle();

      Restaurant userAfterUpdate = _dbContext.Restaurants.SingleOrDefault(u => u.Email == testRestaurant.Email);
      userAfterUpdate.Address.Should().NotBeNull();
    }
  }
}