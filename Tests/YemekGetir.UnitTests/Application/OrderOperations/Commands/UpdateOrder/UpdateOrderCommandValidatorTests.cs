using YemekGetir.Application.OrderOperations.Commands.UpdateOrder;
using FluentAssertions;
using TestSetup;
using Xunit;
using YemekGetir.Common;

namespace Application.OrderOperations.Commands.UpdateOrder
{
  public class UpdateOrderCommandValidatorTests : IClassFixture<CommonTestFixture>
  {
    [Theory]
    // orderId - orderStatus
    // [InlineData(1, 1)] - Valid
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(1, 4)]
    public void WhenInvalidInputsAreGiven_Validator_ShouldReturnErrors(int orderId, int orderStatus)
    {
      // arrange
      UpdateOrderCommand command = new UpdateOrderCommand(null, null, null);
      command.Id = orderId;
      command.Model = new UpdateOrderModel()
      {
        OrderStatus = (StatusEnum)orderStatus
      };

      // act
      UpdateOrderCommandValidator validator = new UpdateOrderCommandValidator();
      var validationResult = validator.Validate(command);

      // assert
      validationResult.Errors.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void WhenValidInputsAreGiven_Validator_ShouldReturnNotErrors()
    {
      // arrange
      UpdateOrderCommand command = new UpdateOrderCommand(null, null, null);
      command.Id = 1;
      command.Model = new UpdateOrderModel()
      {
        OrderStatus = (StatusEnum)2
      };

      // act
      UpdateOrderCommandValidator validator = new UpdateOrderCommandValidator();
      var validationResult = validator.Validate(command);

      // assert
      validationResult.Errors.Count.Should().Be(0);
    }
  }
}