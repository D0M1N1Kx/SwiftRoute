using FluentValidation;
using RouteXY.Api.Requests;

namespace RouteXY.Api.Validators;

public class UpdateInventoryItemRequestValidator : AbstractValidator<UpdateInventoryItemRequest>
{
    public UpdateInventoryItemRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(150)
            .When(x => x.Name != null);
        
        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative")
            .When(x => x.Quantity != null);
        
        RuleFor(x => x.Unit)
            .MaximumLength(30)
            .When(x => x.Unit != null);
    }
}