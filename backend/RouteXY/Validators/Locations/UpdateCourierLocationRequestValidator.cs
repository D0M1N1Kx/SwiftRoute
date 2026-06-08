using FluentValidation;
using RouteXY.Api.Requests;

namespace RouteXY.Api.Validators;

public class UpdateCourierLocationRequestValidator : AbstractValidator<UpdateCourierLocationRequest>
{
    public UpdateCourierLocationRequestValidator() {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude must be between -180 and 180");

        RuleFor(x => x.SpeedKmh)
            .GreaterThanOrEqualTo(0)
            .When(x => x.SpeedKmh.HasValue)
            .WithMessage("Speed cannot be negative");

        RuleFor(x => x.Heading)
            .InclusiveBetween(0, 360)
            .When(x => x.Heading.HasValue)
            .WithMessage("Heading must be between 0 and 360");

        RuleFor(x => x.OrderId)
            .NotEmpty()
            .When(x => x.OrderId.HasValue)
            .WithMessage("OrderId cannot be empty GUID");
    }
}