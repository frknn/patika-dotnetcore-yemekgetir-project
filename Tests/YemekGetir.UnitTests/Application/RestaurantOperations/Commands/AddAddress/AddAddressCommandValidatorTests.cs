using YemekGetir.Application.RestaurantOperations.Commands.AddAddress;
using FluentAssertions;
using TestSetup;
using Xunit;

namespace Application.RestaurantOperations.Commands.AddAddress
{
  public class AddAddressCommandValidatorTests : IClassFixture<CommonTestFixture>
  {
    [Theory]
    // country - city - district - line1 - line2
    // [InlineData("Türkiye","Karabük","Merkez","Satır 1","Satır 2")] - Valid
    [InlineData("", "Karabük", "Merkez", "Satır 1", "Satır 2")]
    [InlineData("Türkiye", "", "Merkez", "Satır 1", "Satır 2")]
    [InlineData("Türkiye", "Karabük", "", "Satır 1", "Satır 2")]
    [InlineData("Türkiye", "Karabük", "Merkez", "", "Satır 2")]
    [InlineData("Türkiye", "Karabük", "Merkez", "Satır 1", "")]

    public void WhenInvalidInputsAreGiven_Validator_ShouldReturnErrors(string country, string city, string district, string line1, string line2)
    {
      // arrange
      AddAddressCommand command = new AddAddressCommand(null, null, null);
      command.Model = new AddAddressToRestaurantModel()
      {
        Country = country,
        District = district,
        City = city,
        Line1 = line1,
        Line2 = line2
      };

      // act
      AddAddressCommandValidator validator = new AddAddressCommandValidator();
      var validationResult = validator.Validate(command);

      // assert
      validationResult.Errors.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void WhenValidInputsAreGiven_Validator_ShouldReturnErrors()
    {
      // arrange
      AddAddressCommand command = new AddAddressCommand(null, null, null);
      command.Model = new AddAddressToRestaurantModel()
      {
        Country = "Türkiye",
        City = "Karabük",
        District = "Merkez",
        Line1 = "Satır 1",
        Line2 = "Satır 2"
      };

      // act
      AddAddressCommandValidator validator = new AddAddressCommandValidator();
      var validationResult = validator.Validate(command);

      // assert
      validationResult.Errors.Count.Should().Be(0);
    }
  }
}