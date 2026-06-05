using FluentValidation;
using RouteXY.Api.Requests;

namespace RouteXY.Api.Validators.Orders;

public class AssignCourierRequestValidator : AbstractValidator<AssignCourierRequest>
{
    public AssignCourierRequestValidator()
    {
        RuleFor(x => x.CourierId)
            .NotEmpty().WithMessage("Courier ID is required");
    }
}