using FluentValidation;
using RouteXY.Api.Requests;

namespace RouteXY.Api.Validators.Orders;

public class UpdateOrderStatusRequestValidator : AbstractValidator<UpdateOrderStatusRequest>
{
    public UpdateOrderStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid order status");

        RuleFor(x => x.Note)
            .MaximumLength(500)
            .When(x => x.Note != null);
    }
}