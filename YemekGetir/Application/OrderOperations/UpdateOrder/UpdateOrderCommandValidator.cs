using FluentValidation;
using YemekGetir.Common;

namespace YemekGetir.Application.OrderOperations.Commands.UpdateOrder
{
  public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
  {
    public UpdateOrderCommandValidator()
    {
      RuleFor(command => command.Id).GreaterThan(0);
      RuleFor(command => command.Model.OrderStatus).IsInEnum();
    }
  }

}