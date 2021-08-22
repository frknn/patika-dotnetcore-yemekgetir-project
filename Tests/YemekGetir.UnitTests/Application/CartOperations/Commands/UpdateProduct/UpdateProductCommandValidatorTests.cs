using YemekGetir.Application.CartOperations.Commands.UpdateProduct;
using FluentAssertions;
using TestSetup;
using Xunit;

namespace Application.CartOperations.Commands.UpdateProduct
{
  public class UpdateProductCommandValidatorTests : IClassFixture<CommonTestFixture>
  {
    [Theory]
    // cartId - lineItemId - quantity
    // [InlineData("1", "1", 1)] - Valid
    [InlineData("1", "1", 0)]
    [InlineData("1", "0", 1)]
    [InlineData("0", "1", 1)]
    public void WhenInvalidInputsAreGiven_Validator_ShouldReturnErrors(string cartId, string lineItemId, int quantity)
    {
      // arrange
      UpdateProductCommand command = new UpdateProductCommand(null, null, null);
      command.CartId = cartId;
      command.LineItemId = lineItemId;
      command.Model = new UpdateProductInCartModel()
      {
        Quantity = quantity
      };

      // act
      UpdateProductCommandValidator validator = new UpdateProductCommandValidator();
      var validationResult = validator.Validate(command);

      // assert
      validationResult.Errors.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void WhenValidInputsAreGiven_Validator_ShouldReturnNotErrors()
    {
      // arrange
      UpdateProductCommand command = new UpdateProductCommand(null, null, null);
      command.CartId = "1";
      command.LineItemId = "1";
      command.Model = new UpdateProductInCartModel()
      {
        Quantity = 1
      };

      // act
      UpdateProductCommandValidator validator = new UpdateProductCommandValidator();
      var validationResult = validator.Validate(command);

      // assert
      validationResult.Errors.Count.Should().Be(0);
    }
  }
}