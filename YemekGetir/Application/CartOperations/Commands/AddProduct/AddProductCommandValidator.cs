using FluentValidation;

namespace YemekGetir.Application.CartOperations.Commands.AddProduct
{
  public class AddProductCommandValidator : AbstractValidator<AddProductCommand>
  {
    public AddProductCommandValidator()
    {
      RuleFor(command => command.Model.ProductId).GreaterThan(0);
      RuleFor(command => command.Model.Quantity).GreaterThan(0);
      RuleFor(command => int.Parse(command.Id)).GreaterThan(0);
    }
  }

}