using YemekGetir.Application.UserOperations.Commands.UpdateAddress;
using FluentAssertions;
using TestSetup;
using Xunit;

namespace Application.UserOperations.Commands.UpdateAddress
{
  public class UpdateAddressCommandValidatorTests : IClassFixture<CommonTestFixture>
  {
    [Fact]
    public void WhenValidInputsAreGiven_Validator_ShouldReturnErrors()
    {
      // arrange
      UpdateAddressCommand command = new UpdateAddressCommand(null, null, null);
      command.Id = "a02e8560-858e-4606-afd9-a0244701d217";
      command.Model = new UpdateUserAddressModel()
      {
        Country = "Türkiye",
        City = "Karabük",
        District = "Merkez",
        Line1 = "Satır 1",
        Line2 = "Satır 2"
      };

      // act
      UpdateAddressCommandValidator validator = new UpdateAddressCommandValidator();
      var validationResult = validator.Validate(command);

      // assert
      validationResult.Errors.Count.Should().Be(0);
    }
  }
}