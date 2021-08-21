using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using YemekGetir.Application.UserOperations.Commands.DeleteUser;
using YemekGetir.DBOperations;
using YemekGetir.Entities;
using TestSetup;
using Xunit;
using YemekGetir.Application.UserOperations.Commands.CreateUser;
using AutoMapper;

namespace Application.UserOperations.Commands.DeleteUser
{
  public class DeleteUserCommandTests : IClassFixture<CommonTestFixture>
  {
    private readonly YemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;
    public DeleteUserCommandTests(CommonTestFixture testFixture)
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

      DeleteUserCommand command = new DeleteUserCommand(_dbContext, mockHttpContextAccessor.Object);
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
      CreateUserCommand createUserCommand = new CreateUserCommand(_dbContext, _mapper);
      createUserCommand.Model = new CreateUserModel()
      {
        FirstName = "userfn",
        LastName = "userln",
        Email = "user@example.com",
        Password = "user123",
        PhoneNumber = "5555555555"
      };

      createUserCommand.Handle();

      User user = _dbContext.Users.SingleOrDefault(user => user.Email == createUserCommand.Model.Email);

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", "1")
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      DeleteUserCommand deleteUserCommand = new DeleteUserCommand(_dbContext, mockHttpContextAccessor.Object);
      deleteUserCommand.Id = user.Id.ToString();

      FluentActions
        .Invoking(() => deleteUserCommand.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Yalnızca kendi hesabınızı silebilirsiniz.");
    }

    [Fact]
    public void WhenValidInputsAreGiven_User_ShouldBeDeleted()
    {
      User customer = new User() { FirstName = "A New", LastName = "User To Delete", Email = "customertodelete@example.com", Password = "customertodelete", };
      _dbContext.Users.Add(customer);
      _dbContext.SaveChanges();

      User newUser = _dbContext.Users.SingleOrDefault(c => c.Email.ToLower() == customer.Email.ToLower());

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", newUser.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      DeleteUserCommand command = new DeleteUserCommand(_dbContext, mockHttpContextAccessor.Object);
      command.Id = newUser.Id.ToString();

      FluentActions.Invoking(() => command.Handle()).Invoke();

      User deletedUser = _dbContext.Users.SingleOrDefault(c => c.Email.ToLower() == newUser.Email.ToLower());

      deletedUser.Should().BeNull();
    }
  }
}
