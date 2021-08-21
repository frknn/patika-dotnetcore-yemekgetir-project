using System;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using YemekGetir.Application.UserOperations.Commands.AddAddress;
using YemekGetir.DBOperations;
using YemekGetir.Entities;
using TestSetup;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;

namespace Application.UserOperations.Commands.AddAddress
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
    public void WhenGivenUserIsNotFound_Handle_ThrowsInvalidOperationException()
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
        .Message.Should().Be("Kullanıcı bulunamadı.");
    }

    [Fact]
    public void WhenGivenUserIdIsNotTheSameWithTheUserIdInToken_Handle_ThrowsInvalidOperationException()
    {
      User user = new User()
      {
        FirstName = "userfn",
        LastName = "userln",
        Email = "user1@example.com",
        Password = "user123",
        PhoneNumber = "5555555555"
      };

      _dbContext.Users.Add(user);
      _dbContext.SaveChanges();
      User newUser = _dbContext.Users.SingleOrDefault(u => u.Email == user.Email);


      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", "1")
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      AddAddressCommand addAddressCommand = new AddAddressCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      addAddressCommand.Id = newUser.Id.ToString();

      FluentActions
        .Invoking(() => addAddressCommand.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Yalnızca kendi hesabınıza adres bilgisi ekleyebilirsiniz.");
    }

    [Fact]
    public void WhenGivenUserAlreadyHasAnAddress_Handle_ThrowsInvalidOperationException()
    {
      Address address = new Address()
      {
        Country = "Türkiye",
        City = "Karabük",
        District = "Merkez",
        Line1 = "Adres satırı 1",
        Line2 = "Adres satırı 2",
      };
      User user = new User()
      {
        FirstName = "userfn",
        LastName = "userln",
        Email = "user12@example.com",
        Password = "user123",
        PhoneNumber = "5555555555",
        Address = address
      };
      _dbContext.Users.Add(user);
      _dbContext.Addresses.Add(address);
      _dbContext.SaveChanges();

      User testUser = _dbContext.Users.SingleOrDefault(u => u.Email == user.Email);

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", testUser.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      AddAddressCommand command = new AddAddressCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.Id = testUser.Id.ToString();
      command.Model = new AddAddressToUserModel()
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
    public void WhenValidInputsAreGiven_Address_ShouldBeAddedToUser()
    {
      User user = new User()
      {
        FirstName = "userfn",
        LastName = "userln",
        Email = "user123@example.com",
        Password = "user123",
        PhoneNumber = "5555555555",
      };
      _dbContext.Users.Add(user);
      _dbContext.SaveChanges();

      User testUser = _dbContext.Users.SingleOrDefault(u => u.Email == user.Email);

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", testUser.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      AddAddressCommand command = new AddAddressCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.Id = testUser.Id.ToString();
      command.Model = new AddAddressToUserModel()
      {
        Country = "Türkiye",
        City = "Karabük",
        District = "Merkez",
        Line1 = "Adres satırı 1",
        Line2 = "Adres satırı 2",
      };

      command.Handle();

      User userAfterUpdate = _dbContext.Users.SingleOrDefault(u => u.Email == testUser.Email);
      userAfterUpdate.Address.Should().NotBeNull();
    }
  }
}