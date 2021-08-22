using YemekGetir.Application.RestaurantOperations.Commands.CreateToken;
using FluentAssertions;
using TestSetup;
using Xunit;

namespace Application.RestaurantOperations.Commands.CreateToken
{
  public class CreateTokenCommandValidatorTests : IClassFixture<CommonTestFixture>
  {
    [Theory]
    // email - password
    // [InlineData("newrestaurant@example.com", "newrestaurant")] - Valid
    [InlineData("", "newrestaurant")]
    [InlineData(" ", "newrestaurant")]
    [InlineData("    ", "newrestaurant")]
    [InlineData("  aaa   ", "newrestaurant")]
    [InlineData("in%va%lid%email.com", "newrestaurant")]
    [InlineData("example.com", "newrestaurant")]
    [InlineData("A@b@c@domain.com", "newrestaurant")]
    [InlineData("newrestaurant@example.com", "")]
    [InlineData("newrestaurant@example.com", " ")]
    [InlineData("newrestaurant@example.com", "   ")]
    [InlineData("newrestaurant@example.com", "a")]
    [InlineData("newrestaurant@example.com", "ab")]
    [InlineData("newrestaurant@example.com", "abc")]
    [InlineData("newrestaurant@example.com", "abcd")]
    [InlineData("newrestaurant@example.com", "abcde")]

    public void WhenInvalidInputsAreGiven_Validator_ShouldReturnErrors(string email, string password)
    {
      // arrange
      CreateTokenCommand command = new CreateTokenCommand(null, null);
      command.Model = new RestaurantLoginModel()
      {
        Email = email,
        Password = password,
      };

      // act
      CreateTokenCommandValidator validator = new CreateTokenCommandValidator();
      var validationResult = validator.Validate(command);

      // assert
      validationResult.Errors.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void WhenValidInputsAreGiven_Validator_ShouldNotReturnError()
    {
      // arrange
      CreateTokenCommand command = new CreateTokenCommand(null, null);
      command.Model = new RestaurantLoginModel()
      {
        Email = "newrestaurant@example.com",
        Password = "newrestaurant",
      };

      // act
      CreateTokenCommandValidator validator = new CreateTokenCommandValidator();
      var validationResult = validator.Validate(command);

      // assert
      validationResult.Errors.Count.Should().Be(0);
    }
  }
}