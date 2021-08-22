using YemekGetir.Application.CartOperations.Commands.AddProduct;
using FluentAssertions;
using TestSetup;
using Xunit;

namespace Application.CartOperations.Commands.AddProduct
{
  public class AddProductCommandValidatorTests : IClassFixture<CommonTestFixture>
  {
    [Theory]
    // cartId - productId - quantity
    // [InlineData(1, 1, 1)] - Valid
    [InlineData(1, 1, 0)]
    [InlineData(1, 0, 1)]
    [InlineData(0, 1, 1)]
    public void WhenInvalidInputsAreGiven_Validator_ShouldReturnErrors(int cartId, int productId, int quantity)
    {
      // arrange
      AddProductCommand command = new AddProductCommand(null, null, null);
      command.Id = cartId;
      command.Model = new AddProductToCartModel()
      {
        ProductId = productId,
        Quantity = quantity
      };

      // act
      AddProductCommandValidator validator = new AddProductCommandValidator();
      var validationResult = validator.Validate(command);

      // assert
      validationResult.Errors.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void WhenValidInputsAreGiven_Validator_ShouldReturnNotErrors()
    {
      // arrange
      AddProductCommand command = new AddProductCommand(null, null, null);
      command.Id = 1;
      command.Model = new AddProductToCartModel()
      {
        ProductId = 1,
        Quantity = 1
      };

      // act
      AddProductCommandValidator validator = new AddProductCommandValidator();
      var validationResult = validator.Validate(command);

      // assert
      validationResult.Errors.Count.Should().Be(0);
    }
  }
}