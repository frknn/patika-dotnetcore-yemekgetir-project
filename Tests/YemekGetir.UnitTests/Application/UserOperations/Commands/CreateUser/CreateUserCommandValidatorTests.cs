using System;
using YemekGetir.Application.UserOperations.Commands.CreateUser;
using FluentAssertions;
using TestSetup;
using Xunit;

namespace Application.UserOperations.Commands.CreateUser
{
  public class CreateUserCommandValidatorTests : IClassFixture<CommonTestFixture>
  {

    [Theory]
    // email - password - firstName - lastName
    // [InlineData("newcustomer@example.com", "newcustomer", "firstname", "lastname")] - Valid
    [InlineData("", "newcustomer", "firstname", "lastname", "5555555555")]
    [InlineData(" ", "newcustomer", "firstname", "lastname", "5555555555")]
    [InlineData("    ", "newcustomer", "firstname", "lastname", "5555555555")]
    [InlineData("  aaa   ", "newcustomer", "firstname", "lastname", "5555555555")]
    [InlineData("in%va%lid%email.com", "newcustomer", "firstname", "lastname", "5555555555")]
    [InlineData("example.com", "newcustomer", "firstname", "lastname", "5555555555")]
    [InlineData("A@b@c@domain.com", "newcustomer", "firstname", "lastname", "5555555555")]
    [InlineData("newcustomer@example.com", "", "firstname", "lastname", "5555555555")]
    [InlineData("newcustomer@example.com", " ", "firstname", "lastname", "5555555555")]
    [InlineData("newcustomer@example.com", "   ", "firstname", "lastname", "5555555555")]
    [InlineData("newcustomer@example.com", "a", "firstname", "lastname", "5555555555")]
    [InlineData("newcustomer@example.com", "ab", "firstname", "lastname", "5555555555")]
    [InlineData("newcustomer@example.com", "abc", "firstname", "lastname", "5555555555")]
    [InlineData("newcustomer@example.com", "abcd", "firstname", "lastname", "5555555555")]
    [InlineData("newcustomer@example.com", "abcde", "firstname", "lastname", "5555555555")]
    [InlineData("newcustomer@example.com", "newcustomer", "", "lastname", "5555555555")]
    [InlineData("newcustomer@example.com", "newcustomer", " ", "lastname", "5555555555")]
    [InlineData("newcustomer@example.com", "newcustomer", "firstname", "", "5555555555")]
    [InlineData("newcustomer@example.com", "newcustomer", "firstname", " ", "5555555555")]
    [InlineData("newcustomer@example.com", "newcustomer", "firstname", "lastname", "555")]
    public void WhenInvalidInputsAreGiven_Validator_ShouldReturnErrors(string email, string password, string firstName, string lastName, string phoneNumber)
    {
      // arrange
      CreateUserCommand command = new CreateUserCommand(null, null);
      command.Model = new CreateUserModel()
      {
        Email = email,
        Password = password,
        FirstName = firstName,
        LastName = lastName,
        PhoneNumber = phoneNumber
      };

      // act
      CreateUserCommandValidator validator = new CreateUserCommandValidator();
      var validationResult = validator.Validate(command);

      // assert
      validationResult.Errors.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void WhenValidInputsAreGiven_Validator_ShouldNotReturnError()
    {
      // arrange
      CreateUserCommand command = new CreateUserCommand(null, null);
      command.Model = new CreateUserModel()
      {
        Email = "newcustomer@example.com",
        Password = "newcustomer",
        FirstName = "firstname",
        LastName = "lastname",
        PhoneNumber = "5555555555"
      };

      // act
      CreateUserCommandValidator validator = new CreateUserCommandValidator();
      var validationResult = validator.Validate(command);

      // assert
      validationResult.Errors.Count.Should().Be(0);
    }
  }
}