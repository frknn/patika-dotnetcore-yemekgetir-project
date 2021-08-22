using System;
using YemekGetir.Application.RestaurantOperations.Commands.CreateRestaurant;
using FluentAssertions;
using TestSetup;
using Xunit;

namespace Application.RestaurantOperations.Commands.CreateRestaurant
{
  public class CreateRestaurantCommandValidatorTests : IClassFixture<CommonTestFixture>
  {

    [Theory]
    // email - password - name - categoryId
    // [InlineData("newrest@example.com", "newrest", "firstname", "lastname")] - Valid
    [InlineData("", "newrest", "newrestname", 1)]
    [InlineData(" ", "newrest", "newrestname", 1)]
    [InlineData("    ", "newrest", "newrestname", 1)]
    [InlineData("  aaa   ", "newrest", "newrestname", 1)]
    [InlineData("in%va%lid%email.com", "newrest", "newrestname", 1)]
    [InlineData("example.com", "newrest", "newrestname", 1)]
    [InlineData("A@b@c@domain.com", "newrest", "newrestname", 1)]
    [InlineData("newrest@example.com", "", "newrestname", 1)]
    [InlineData("newrest@example.com", " ", "newrestname", 1)]
    [InlineData("newrest@example.com", "   ", "newrestname", 1)]
    [InlineData("newrest@example.com", "a", "newrestname", 1)]
    [InlineData("newrest@example.com", "ab", "newrestname", 1)]
    [InlineData("newrest@example.com", "abc", "newrestname", 1)]
    [InlineData("newrest@example.com", "abcd", "newrestname", 1)]
    [InlineData("newrest@example.com", "abcde", "newrestname", 1)]
    [InlineData("newrest@example.com", "newrest", "", 1)]
    [InlineData("newrest@example.com", "newrest", " ", 1)]
    [InlineData("newrest@example.com", "newrest", "  ", 1)]
    [InlineData("newrest@example.com", "newrest", "newrestname", 0)]
    public void WhenInvalidInputsAreGiven_Validator_ShouldReturnErrors(string email, string password, string name, int categoryId)
    {
      // arrange
      CreateRestaurantCommand command = new CreateRestaurantCommand(null, null);
      command.Model = new CreateRestaurantModel()
      {
        Email = email,
        Password = password,
        Name = name,
        CategoryId = categoryId
      };

      // act
      CreateRestaurantCommandValidator validator = new CreateRestaurantCommandValidator();
      var validationResult = validator.Validate(command);

      // assert
      validationResult.Errors.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void WhenValidInputsAreGiven_Validator_ShouldNotReturnError()
    {
      // arrange
      CreateRestaurantCommand command = new CreateRestaurantCommand(null, null);
      command.Model = new CreateRestaurantModel()
      {
        Email = "newrest@example.com",
        Password = "newrest",
        Name = "New Rest",
        CategoryId = 1,
      };

      // act
      CreateRestaurantCommandValidator validator = new CreateRestaurantCommandValidator();
      var validationResult = validator.Validate(command);

      // assert
      validationResult.Errors.Count.Should().Be(0);
    }
  }
}