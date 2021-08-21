using FluentValidation;

namespace YemekGetir.Application.RestaurantOperations.Commands.UpdateProduct
{
  public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
  {
    public UpdateProductCommandValidator()
    {
      RuleFor(command => int.Parse(command.ProductId)).GreaterThan(0);
      RuleFor(command => command.RestaurantId).NotEmpty();
      RuleFor(command => command.Model.Name).MinimumLength(1).When(command => command.Model.Name != string.Empty);
      RuleFor(command => command.Model.Price).GreaterThan(-1);

    }
  }

}