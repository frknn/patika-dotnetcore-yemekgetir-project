using YemekGetir.Application.CartOperations.Commands.EmptyCart;
using FluentAssertions;
using TestSetup;
using Xunit;

namespace Application.CartOperations.Commands.EmptyCart
{
  public class EmptyCartCommandValidatorTests : IClassFixture<CommonTestFixture>
  {
    [Theory]
    // cartId
    // [InlineData("1")] - Valid
    [InlineData("0")]
    public void WhenInvalidInputsAreGiven_Validator_ShouldReturnErrors(string cartId)
    {
      // arrange
      EmptyCartCommand command = new EmptyCartCommand(null, null);
      command.Id = cartId;

      // act
      EmptyCartCommandValidator validator = new EmptyCartCommandValidator();
      var validationResult = validator.Validate(command);

      // assert
      validationResult.Errors.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void WhenValidInputsAreGiven_Validator_ShouldReturnNotErrors()
    {
      // arrange
      EmptyCartCommand command = new EmptyCartCommand(null, null);
      command.Id = "1";

      // act
      EmptyCartCommandValidator validator = new EmptyCartCommandValidator();
      var validationResult = validator.Validate(command);

      // assert
      validationResult.Errors.Count.Should().Be(0);
    }
  }
}