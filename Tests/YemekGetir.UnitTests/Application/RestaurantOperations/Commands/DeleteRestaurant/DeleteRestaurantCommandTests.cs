using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using YemekGetir.Application.RestaurantOperations.Commands.DeleteRestaurant;
using YemekGetir.DBOperations;
using YemekGetir.Entities;
using TestSetup;
using Xunit;
using YemekGetir.Application.RestaurantOperations.Commands.CreateRestaurant;
using AutoMapper;

namespace Application.RestaurantOperations.Commands.DeleteRestaurant
{
  public class DeleteRestaurantCommandTests : IClassFixture<CommonTestFixture>
  {
    private readonly YemekGetirDbContext _dbContext;
    private readonly IMapper _mapper;
    public DeleteRestaurantCommandTests(CommonTestFixture testFixture)
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

      DeleteRestaurantCommand command = new DeleteRestaurantCommand(_dbContext, mockHttpContextAccessor.Object);
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
      CreateRestaurantCommand createRestaurantCommand = new CreateRestaurantCommand(_dbContext, _mapper);
      createRestaurantCommand.Model = new CreateRestaurantModel()
      {
        Name = "New Restaurant 4656468",
        Email = "newrest546684865@example.com",
        Password = "newrest54645",
        CategoryId = 1,
      };

      createRestaurantCommand.Handle();

      Restaurant restaurant = _dbContext.Restaurants.SingleOrDefault(restaurant => restaurant.Email == createRestaurantCommand.Model.Email);

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", "1")
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      DeleteRestaurantCommand deleteRestaurantCommand = new DeleteRestaurantCommand(_dbContext, mockHttpContextAccessor.Object);
      deleteRestaurantCommand.Id = restaurant.Id.ToString();

      FluentActions
        .Invoking(() => deleteRestaurantCommand.Handle())
        .Should().Throw<InvalidOperationException>()
        .And
        .Message.Should().Be("Yalnızca kendi hesabınızı silebilirsiniz.");
    }

    [Fact]
    public void WhenValidInputsAreGiven_Restaurant_ShouldBeDeleted()
    {
      Restaurant restaurant = new Restaurant() { Name = "A New Restaurant 6465465", Email = "newrestotodelete@example.com", Password = "restotodelete", CategoryId = 1};
      _dbContext.Restaurants.Add(restaurant);
      _dbContext.SaveChanges();

      Restaurant newRestaurant = _dbContext.Restaurants.SingleOrDefault(r => r.Email.ToLower() == restaurant.Email.ToLower());

      var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var claims = new List<Claim>()
      {
          new Claim("tokenHolderId", newRestaurant.Id.ToString())
      };
      mockHttpContextAccessor.Setup(accessor => accessor.HttpContext.User.Claims).Returns(claims);

      DeleteRestaurantCommand command = new DeleteRestaurantCommand(_dbContext, mockHttpContextAccessor.Object);
      command.Id = newRestaurant.Id.ToString();

      FluentActions.Invoking(() => command.Handle()).Invoke();

      Restaurant deletedRestaurant = _dbContext.Restaurants.SingleOrDefault(c => c.Email.ToLower() == newRestaurant.Email.ToLower());

      deletedRestaurant.Should().BeNull();
    }
  }
}
