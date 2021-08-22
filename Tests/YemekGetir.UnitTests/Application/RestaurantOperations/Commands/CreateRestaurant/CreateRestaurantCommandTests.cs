using System;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using YemekGetir.Application.RestaurantOperations.Commands.CreateRestaurant;
using YemekGetir.DBOperations;
using YemekGetir.Entities;
using TestSetup;
using Xunit;

namespace Application.RestaurantOperations.Commands.CreateRestaurant
{
  public class CreateRestaurantCommandTests : IClassFixture<CommonTestFixture>
  {
    private readonly YemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;

    public CreateRestaurantCommandTests(CommonTestFixture testFixture)
    {
      _dbContext = testFixture.Context;
      _mapper = testFixture.Mapper;
    }

    [Fact]
    public void WhenAlreadyExistingRestaurantEmailIsGiven_Handle_ThrowsInvalidOperationException()
    {
      Restaurant Restaurant = new Restaurant()
      {
        Email = "existing12@example.com",
        Password = BCrypt.Net.BCrypt.HashPassword("existing123"),
        Name = "existingfn",
        CategoryId = 1
      };
      _dbContext.Restaurants.Add(Restaurant);
      _dbContext.SaveChanges();

      // arrange
      CreateRestaurantCommand command = new CreateRestaurantCommand(_dbContext, _mapper);
      command.Model = new CreateRestaurantModel()
      {
        Email = "existing12@example.com",
        Password = BCrypt.Net.BCrypt.HashPassword("existing123"),
        Name = "existingfn",
        CategoryId = 1
      };

      // act & assert
      FluentActions
        .Invoking(() => command.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Restoran zaten mevcut.");
    }

    [Fact]
    public void WhenValidInputsAreGiven_Restaurant_ShouldBeCreated()
    {
      // arrange
      CreateRestaurantCommand command = new CreateRestaurantCommand(_dbContext, _mapper);
      var model = new CreateRestaurantModel()
      {
        Email = "existing123@example.com",
        Password = BCrypt.Net.BCrypt.HashPassword("existing123"),
        Name = "existingfn",
        CategoryId = 1
      };
      command.Model = model;

      // act
      FluentActions.Invoking(() => command.Handle()).Invoke();

      // assert
      var Restaurant = _dbContext.Restaurants.SingleOrDefault(Restaurant => Restaurant.Email.ToLower() == model.Email.ToLower());

      Restaurant.Should().NotBeNull();
    }
  }
}