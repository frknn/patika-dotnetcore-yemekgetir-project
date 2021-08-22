using System;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using YemekGetir.Application.RestaurantOperations.Commands.CreateRestaurant;
using YemekGetir.Application.RestaurantOperations.Commands.CreateToken;
using YemekGetir.DBOperations;
using YemekGetir.Entities;
using TestSetup;
using Xunit;

namespace Application.RestaurantOperations.Commands.CreateToken
{
  public class CreateTokenCommandTests : IClassFixture<CommonTestFixture>
  {
    private readonly YemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;

    private readonly IConfiguration _configuration;

    public CreateTokenCommandTests(CommonTestFixture testFixture)
    {
      _dbContext = testFixture.Context;
      _mapper = testFixture.Mapper;
      _configuration = testFixture.Configuration;
    }

    [Theory]
    [InlineData("nonexisting@example.com", "nonexisting")]
    [InlineData("furkan123@example.com", "nonexisting")]
    [InlineData("nonexisting@example.com", "furkan123")]
    public void WhenInvalidCredentialsAreGiven_Handle_ThrowsInvalidOperationException(string email, string password)
    {
      // arrange
      CreateTokenCommand command = new CreateTokenCommand(_dbContext, _configuration);
      command.Model = new RestaurantLoginModel()
      {
        Email = email,
        Password = password,
      };

      // act & assert
      FluentActions
        .Invoking(() => command.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Kulanıcı adı veya şifre yanlış.");
    }

    [Fact]
    public void WhenValidInputsAreGiven_RestaurantToken_ShouldBeCreated()
    {
      // arrange
      var customerModel = new CreateRestaurantModel()
      {
        Email = "newresttotesttoken@example.com",
        Password = "newresttotesttoken",
        Name = "New Rest",
        CategoryId = 1
      };

      var customer = _mapper.Map<Restaurant>(customerModel);
      _dbContext.Restaurants.Add(customer);
      _dbContext.SaveChanges();

      CreateTokenCommand command = new CreateTokenCommand(_dbContext, _configuration);
      command.Model = new RestaurantLoginModel()
      {
        Email = customerModel.Email,
        Password = customerModel.Password,
      };

      // act & assert

      var token = command.Handle();
      token.Should().NotBeNull();
      token.AccessToken.Should().NotBeNull();
      token.RefreshToken.Should().NotBeNull();
      token.ExpirationDate.Should().BeAfter(DateTime.Now);

    }
  }
}