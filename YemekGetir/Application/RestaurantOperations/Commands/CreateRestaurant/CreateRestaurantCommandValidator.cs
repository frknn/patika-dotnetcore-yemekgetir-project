using FluentValidation;

namespace YemekGetir.Application.RestaurantOperations.Commands.CreateRestaurant
{
  public class CreateRestaurantCommandValidator : AbstractValidator<CreateRestaurantCommand>
  {
    public CreateRestaurantCommandValidator()
    {
      RuleFor(command => command.Model.Name).NotEmpty().MinimumLength(1);
      RuleFor(command => command.Model.CategoryId).GreaterThan(0);
      RuleFor(command => command.Model.Email).NotEmpty().MinimumLength(4).EmailAddress();
      RuleFor(command => command.Model.Password).NotEmpty().MinimumLength(6);
    }
  }

}