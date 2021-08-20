using FluentValidation;

namespace YemekGetir.Application.CartOperations.Commands.UpdateProduct
{
  public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
  {
    public UpdateProductCommandValidator()
    {
      RuleFor(command => command.Model.Quantity).GreaterThan(0);
      RuleFor(command => int.Parse(command.LineItemId)).GreaterThan(0);
      RuleFor(command => int.Parse(command.CartId)).GreaterThan(0);
    }
  }

}