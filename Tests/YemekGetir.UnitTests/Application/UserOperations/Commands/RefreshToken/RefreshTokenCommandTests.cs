using System;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using YemekGetir.Application.UserOperations.Commands.CreateUser;
using YemekGetir.Application.UserOperations.Commands.CreateToken;
using YemekGetir.Application.UserOperations.Commands.RefreshToken;
using YemekGetir.DBOperations;
using YemekGetir.Entities;
using TestSetup;
using Xunit;

namespace Application.UserOperations.Commands.RefreshToken
{
  public class RefreshTokenCommandTests : IClassFixture<CommonTestFixture>
  {
    private readonly YemekGetirDbContext _dbContext;
    private readonly IConfiguration _configuration;

    private readonly IMapper _mapper;

    public RefreshTokenCommandTests(CommonTestFixture testFixture)
    {
      _dbContext = testFixture.Context;
      _configuration = testFixture.Configuration;
      _mapper = testFixture.Mapper;
    }

    [Fact]
    public void WhenValidInputsAreGiven_AccessToken_ShouldBeCreatedWithRefreshToken()
    {
      // arrange
      CreateUserModel userModel = new CreateUserModel()
      {
        Email = "usertotestrefreshtoken@example.com",
        Password = "usertotestrefreshtoken123",
        FirstName = "usertotestrefreshtoken",
        LastName = "userlntotestrefreshtoken",
      };

      User user = _mapper.Map<User>(userModel);
      _dbContext.Users.Add(user);
      _dbContext.SaveChanges();

      CreateTokenCommand command = new CreateTokenCommand(_dbContext, _configuration);
      command.Model = new UserLoginModel()
      {
        Email = userModel.Email,
        Password = userModel.Password
      };

      // act & assert

      var token = command.Handle();

      RefreshTokenCommand refreshTokenCommand = new RefreshTokenCommand(_dbContext, _configuration);
      refreshTokenCommand.RefreshToken = token.RefreshToken;

      var newToken = refreshTokenCommand.Handle();

      newToken.Should().NotBeNull();
      newToken.AccessToken.Should().NotBeNull();
      newToken.RefreshToken.Should().NotBeNull();
      newToken.RefreshToken.Should().NotBe(token.RefreshToken);
      newToken.ExpirationDate.Should().BeAfter(token.ExpirationDate);
    }

    [Fact]
    public void WhenInvalidInputsAreGiven_Handle_ThrowsInvalidOperationException()
    {
      RefreshTokenCommand command = new RefreshTokenCommand(_dbContext, _configuration);
      command.RefreshToken = "invalid refresh token";

      FluentActions
        .Invoking(() => command.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Geçerli bir Refresh Token bulunamadı.");
    }
  }
}