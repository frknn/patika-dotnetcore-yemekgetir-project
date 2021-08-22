using System;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using YemekGetir.Application.UserOperations.Commands.UpdateAddress;
using YemekGetir.DBOperations;
using YemekGetir.Entities;
using TestSetup;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Application.UserOperations.Commands.UpdateAddress
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
    public void WhenGivenUserIsNotFound_Handle_ThrowsInvalidOperationException()
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
        .Message.Should().Be("Kullanıcı bulunamadı.");
    }

    [Fact]
    public void WhenGivenUserIdIsNotTheSameWithTheUserIdInToken_Handle_ThrowsInvalidOperationException()
    {
      User user = new User()
      {
        FirstName = "userfn",
        LastName = "userln",
        Email = "user15454@example.com",
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

      UpdateAddressCommand addAddressCommand = new UpdateAddressCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      addAddressCommand.Id = newUser.Id.ToString();

      FluentActions
        .Invoking(() => addAddressCommand.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Yalnızca kendi hesabınızın adres bilgisini güncelleyebilirsiniz.");
    }

    [Fact]
    public void WhenGivenUserHasNoAddress_Handle_ThrowsInvalidOperationException()
    {
      User user = new User()
      {
        FirstName = "userfn",
        LastName = "userln",
        Email = "user12465465@example.com",
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

      UpdateAddressCommand command = new UpdateAddressCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.Id = testUser.Id.ToString();
      command.Model = new UpdateUserAddressModel()
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
    public void WhenValidInputsAreGiven_Address_ShouldBeUpdated()
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
        Email = "user1234654646545@example.com",
        Password = "user123",
        PhoneNumber = "5555555555",
        Address = address
      };

      _dbContext.Users.Add(user);
      _dbContext.Addresses.Add(address);
      _dbContext.SaveChanges();

      User testUser = _dbContext.Users.Include(user => user.Address).SingleOrDefault(u => u.Email == user.Email);

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", testUser.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      UpdateAddressCommand command = new UpdateAddressCommand(_dbContext, _mapper, mockHttpContextAccessor.Object);
      command.Id = testUser.Id.ToString();
      var model = new UpdateUserAddressModel()
      {
        Country = "Updated Country",
        City = "Updated City",
        District = "Updated District",
        Line1 = "Updated Line 1",
        Line2 = "Updated Line 2",
      };
      command.Model = model;

      command.Handle();

      User userAfterUpdate = _dbContext.Users.Include(user => user.Address).SingleOrDefault(u => u.Email == testUser.Email);
      
      userAfterUpdate.Address.Should().NotBeNull();
      userAfterUpdate.Address.Country.Should().Be(model.Country);
      userAfterUpdate.Address.City.Should().Be(model.City);
      userAfterUpdate.Address.District.Should().Be(model.District);
      userAfterUpdate.Address.Line1.Should().Be(model.Line1);
      userAfterUpdate.Address.Line2.Should().Be(model.Line2);
    }
  }
}