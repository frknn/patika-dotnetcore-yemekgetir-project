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
using YemekGetir.Application.UserOperations.Queries.GetUserById;

namespace Application.UserOperations.Queries.GetUserById
{
  public class GetUserByIdQueryTests : IClassFixture<CommonTestFixture>
  {
    private readonly YemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetUserByIdQueryTests(CommonTestFixture testFixture)
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

      GetUserByIdQuery query = new GetUserByIdQuery(_dbContext, _mapper, mockHttpContextAccessor.Object);
      query.Id = "999";

      FluentActions
        .Invoking(() => query.Handle())
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
        Email = "user146654484854684@example.com",
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

      GetUserByIdQuery addAddressCommand = new GetUserByIdQuery(_dbContext, _mapper, mockHttpContextAccessor.Object);
      addAddressCommand.Id = newUser.Id.ToString();

      FluentActions
        .Invoking(() => addAddressCommand.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Yalnızca kendi hesap bilgilerinizi görüntüleyebilirsiniz.");
    }

    [Fact]
    public void WhenValidInputsAreGiven_Address_ShouldBeAddedToUser()
    {
      User user = new User()
      {
        FirstName = "userfn",
        LastName = "userln",
        Email = "user125448787873@example.com",
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

      GetUserByIdQuery query = new GetUserByIdQuery(_dbContext, _mapper, mockHttpContextAccessor.Object);
      query.Id = testUser.Id.ToString();

      GetUserByIdViewModel foundUser = query.Handle();

      foundUser.Should().NotBeNull();
    }
  }
}