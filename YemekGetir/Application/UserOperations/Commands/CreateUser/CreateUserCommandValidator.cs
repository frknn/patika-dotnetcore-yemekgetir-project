using FluentValidation;

namespace YemekGetir.Application.UserOperations.Commands.CreateUser
{
  public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
  {
    public CreateUserCommandValidator()
    {
      RuleFor(command => command.Model.FirstName).NotEmpty().MinimumLength(1);
      RuleFor(command => command.Model.LastName).NotEmpty().MinimumLength(1);
      RuleFor(command => command.Model.PhoneNumber).NotEmpty().Length(10);
      RuleFor(command => command.Model.Email).NotEmpty().MinimumLength(4).EmailAddress();
      RuleFor(command => command.Model.Password).NotEmpty().MinimumLength(6);
    }
  }

}