using FluentValidation;
using RouteXY.Api.Requests;

namespace RouteXY.Api.Validators.Inventory;

public class CreateInventoryItemRequestValidator : AbstractValidator<CreateInventoryItemRequest>
{
    public CreateInventoryItemRequestValidator()
    {
        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("Warehouse ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Item name is required")
            .MaximumLength(150);

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative");

        RuleFor(x => x.Unit)
            .MaximumLength(30)
            .When(x => x.Unit != null);
    }
}