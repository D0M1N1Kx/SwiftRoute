using FluentValidation;
using RouteXY.Api.Requests;

namespace RouteXY.Api.Validators.Orders;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.RecipientName)
            .NotEmpty().WithMessage("Recipient name is required")
            .MaximumLength(100);

        RuleFor(x => x.RecipientPhone)
            .NotEmpty().WithMessage("Recipient phone is required")
            .Matches(@"^\+?[0-9]{7,15}$").WithMessage("Invalid phone number format");

        RuleFor(x => x.PickupAddress)
            .NotEmpty().WithMessage("Pickup address is required")
            .MaximumLength(255);

        RuleFor(x => x.PickupLat)
            .InclusiveBetween(-90, 90).WithMessage("Invalid pickup latitude");

        RuleFor(x => x.PickupLng)
            .InclusiveBetween(-180, 180).WithMessage("Invalid pickup longitude");

        RuleFor(x => x.DeliveryAddress)
            .NotEmpty().WithMessage("Delivery address is required")
            .MaximumLength(255);

        RuleFor(x => x.DeliveryLat)
            .InclusiveBetween(-90, 90).WithMessage("Invalid delivery latitude");

        RuleFor(x => x.DeliveryLng)
            .InclusiveBetween(-180, 180).WithMessage("Invalid delivery longitude");
    }
}