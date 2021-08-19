using FluentValidation;

namespace YemekGetir.Application.UserOperations.Commands.AddAddress
{
  public class AddAddressCommandValidator : AbstractValidator<AddAddressCommand>
  {
    public AddAddressCommandValidator()
    {
      RuleFor(command => command.Model.Country).NotEmpty().MinimumLength(1);
      RuleFor(command => command.Model.District).NotEmpty().MinimumLength(1);
      RuleFor(command => command.Model.City).NotEmpty().MinimumLength(1);
      RuleFor(command => command.Model.Line1).NotEmpty().MinimumLength(1);
      RuleFor(command => command.Model.Line2).NotEmpty().MinimumLength(1);
    }
  }

}