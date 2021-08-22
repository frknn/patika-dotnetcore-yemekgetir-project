using YemekGetir.Application.RestaurantOperations.Commands.UpdateProduct;
using FluentAssertions;
using TestSetup;
using Xunit;

namespace Application.RestaurantOperations.Commands.UpdateProduct
{
  public class UpdateProductCommandValidatorTests : IClassFixture<CommonTestFixture>
  {
    [Theory]
    // [InlineData("a02e8560-858e-4606-afd9-a0244701d212","1",0)] - Valid
    [InlineData("a02e8560-858e-4606-afd9-a0244701d212", "1", -1)]
    [InlineData("a02e8560-858e-4606-afd9-a0244701d212", "0", 0)]
    [InlineData("", "1", 0)]
    public void WhenInvalidInputsAreGiven_Validator_ShouldNotReturnErrors(string restaurantId, string productId, int price)
    {
      // arrange
      UpdateProductCommand command = new UpdateProductCommand(null, null, null);
      command.RestaurantId = restaurantId;
      command.ProductId = productId;
      command.Model = new UpdateProductModel()
      {
        Name = "Valid Name",
        Price = price
      };

      // act
      UpdateProductCommandValidator validator = new UpdateProductCommandValidator();
      var validationResult = validator.Validate(command);

      // assert
      validationResult.Errors.Count.Should().BeGreaterThan(0);
    }


    [Fact]
    public void WhenValidInputsAreGiven_Validator_ShouldNotReturnErrors()
    {
      // arrange
      UpdateProductCommand command = new UpdateProductCommand(null, null, null);
      command.RestaurantId = "a02e8560-858e-4606-afd9-a0244701d211";
      command.ProductId = "1";
      command.Model = new UpdateProductModel()
      {
        Name = "A Valid Product Name To Test Update",
        Price = 25,
      };

      // act
      UpdateProductCommandValidator validator = new UpdateProductCommandValidator();
      var validationResult = validator.Validate(command);

      // assert
      validationResult.Errors.Count.Should().Be(0);
    }
  }
}