using FluentValidation;

namespace YemekGetir.Application.CartOperations.Commands.EmptyCart
{
  public class EmptyCartCommandValidator : AbstractValidator<EmptyCartCommand>
  {
    public EmptyCartCommandValidator()
    {
      RuleFor(command => int.Parse(command.Id)).GreaterThan(0);
    }
  }

}