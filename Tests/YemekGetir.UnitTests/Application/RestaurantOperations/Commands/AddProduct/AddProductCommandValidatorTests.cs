using YemekGetir.Application.RestaurantOperations.Commands.AddProduct;
using FluentAssertions;
using TestSetup;
using Xunit;

namespace Application.RestaurantOperations.Commands.AddProduct
{
  public class AddProductCommandValidatorTests : IClassFixture<CommonTestFixture>
  {
    [Theory]
    // name - price
    // [InlineData("Rest Name", 10)] - Valid
    [InlineData("", 10)]
    [InlineData("Rest Name", 0)]
    public void WhenInvalidInputsAreGiven_Validator_ShouldReturnErrors(string name, int price)
    {
      // arrange
      AddProductCommand command = new AddProductCommand(null, null, null);
      command.Model = new AddProductModel()
      {
        Name = name,
        Price = price
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
      command.Model = new AddProductModel()
      {
        Name = "A Valid Product Name",
        Price = 20
      };

      // act
      AddProductCommandValidator validator = new AddProductCommandValidator();
      var validationResult = validator.Validate(command);

      // assert
      validationResult.Errors.Count.Should().Be(0);
    }
  }
}