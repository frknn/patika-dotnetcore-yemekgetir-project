using System;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using YemekGetir.Application.UserOperations.Commands.CreateUser;
using YemekGetir.DBOperations;
using YemekGetir.Entities;
using TestSetup;
using Xunit;

namespace Application.UserOperations.Commands.CreateUser
{
  public class CreateUserCommandTests : IClassFixture<CommonTestFixture>
  {
    private readonly YemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;

    public CreateUserCommandTests(CommonTestFixture testFixture)
    {
      _dbContext = testFixture.Context;
      _mapper = testFixture.Mapper;
    }

    [Fact]
    public void WhenAlreadyExistingUserEmailIsGiven_Handle_ThrowsInvalidOperationException()
    {
      User User = new User()
      {
        Email = "existing@example.com",
        Password = BCrypt.Net.BCrypt.HashPassword("existing123"),
        FirstName = "existingfn",
        LastName = "existingln",

      };
      _dbContext.Users.Add(User);
      _dbContext.SaveChanges();

      // arrange
      CreateUserCommand command = new CreateUserCommand(_dbContext, _mapper);
      command.Model = new CreateUserModel()
      {
        Email = "existing@example.com",
        Password = "existing123",
        FirstName = "existingfn",
        LastName = "existingln",
      };

      // act & assert
      FluentActions
        .Invoking(() => command.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Kullanıcı zaten mevcut.");
    }

    // [Fact]
    // public void WhenValidInputsAreGiven_User_ShouldBeCreated()
    // {
    //   // arrange
    //   CreateUserCommand command = new CreateUserCommand(_dbContext, _mapper);
    //   var model = new CreateUserModel()
    //   {
    //     Email = "new@example.com",
    //     Password = "new123123",
    //     FirstName = "newUser",
    //     LastName = "newUserln",
    //   };
    //   command.Model = model;

    //   // act
    //   FluentActions.Invoking(() => command.Handle()).Invoke();

    //   // assert
    //   var User = _dbContext.Users.SingleOrDefault(User => User.Email.ToLower() == model.Email.ToLower());

    //   User.Should().NotBeNull();
    // }
  }
}