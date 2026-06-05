using FluentValidation;
using RouteXY.Api.Requests;

namespace RouteXY.Api.Validators.Warehouses;

public class CreateWarehouseRequestValidator : AbstractValidator<CreateWarehouseRequest>
{
    public CreateWarehouseRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Warehouse name is required")
            .MaximumLength(100);

        RuleFor(x => x.City)
            .MaximumLength(100)
            .When(x => x.City != null);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Invalid latitude")
            .When(x => x.Latitude.HasValue);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Invalid longitude")
            .When(x => x.Longitude.HasValue);
    }
}