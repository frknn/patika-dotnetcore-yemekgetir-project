using YemekGetir.Application.UserOperations.Commands.CreateToken;
using FluentAssertions;
using TestSetup;
using Xunit;

namespace Application.UserOperations.Commands.CreateToken
{
  public class CreateTokenCommandValidatorTests : IClassFixture<CommonTestFixture>
  {
    [Theory]
    // email - password
    // [InlineData("newcustomer@example.com", "newcustomer")] - Valid
    [InlineData("", "newcustomer")]
    [InlineData(" ", "newcustomer")]
    [InlineData("    ", "newcustomer")]
    [InlineData("  aaa   ", "newcustomer")]
    [InlineData("in%va%lid%email.com", "newcustomer")]
    [InlineData("example.com", "newcustomer")]
    [InlineData("A@b@c@domain.com", "newcustomer")]
    [InlineData("newcustomer@example.com", "")]
    [InlineData("newcustomer@example.com", " ")]
    [InlineData("newcustomer@example.com", "   ")]
    [InlineData("newcustomer@example.com", "a")]
    [InlineData("newcustomer@example.com", "ab")]
    [InlineData("newcustomer@example.com", "abc")]
    [InlineData("newcustomer@example.com", "abcd")]
    [InlineData("newcustomer@example.com", "abcde")]

    public void WhenInvalidInputsAreGiven_Validator_ShouldReturnErrors(string email, string password)
    {
      // arrange
      CreateTokenCommand command = new CreateTokenCommand(null, null);
      command.Model = new UserLoginModel()
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
      command.Model = new UserLoginModel()
      {
        Email = "newcustomer@example.com",
        Password = "newcustomer",
      };

      // act
      CreateTokenCommandValidator validator = new CreateTokenCommandValidator();
      var validationResult = validator.Validate(command);

      // assert
      validationResult.Errors.Count.Should().Be(0);
    }
  }
}