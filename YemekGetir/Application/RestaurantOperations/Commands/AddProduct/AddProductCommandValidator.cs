using FluentValidation;

namespace YemekGetir.Application.RestaurantOperations.Commands.AddProduct
{
  public class AddProductCommandValidator : AbstractValidator<AddProductCommand>
  {
    public AddProductCommandValidator()
    {
      RuleFor(command => command.Model.Name).NotEmpty().MinimumLength(1);
      RuleFor(command => command.Model.Price).GreaterThan(0);
    }
  }

}