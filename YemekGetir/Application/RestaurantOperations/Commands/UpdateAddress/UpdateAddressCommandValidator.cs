using FluentValidation;

namespace YemekGetir.Application.RestaurantOperations.Commands.UpdateAddress
{
  public class UpdateAddressCommandValidator : AbstractValidator<UpdateAddressCommand>
  {
    public UpdateAddressCommandValidator()
    {
      RuleFor(command => command.Model.Country).MinimumLength(1).When(command => command.Model.Country != string.Empty);
      RuleFor(command => command.Model.District).MinimumLength(1).When(command => command.Model.District != string.Empty); ;
      RuleFor(command => command.Model.City).MinimumLength(1).When(command => command.Model.City != string.Empty); ;
      RuleFor(command => command.Model.Line1).MinimumLength(1).When(command => command.Model.Line1 != string.Empty); ;
      RuleFor(command => command.Model.Line2).MinimumLength(1).When(command => command.Model.Line2 != string.Empty); ;
    }
  }

}