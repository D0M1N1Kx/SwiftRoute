using FluentValidation;
using RouteXY.Api.Requests;

namespace RouteXY.Api.Validators.Users;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.FullName)
            .MinimumLength(2)
            .MaximumLength(100)
            .When(x => x.FullName != null);
        
        RuleFor(x => x.Phone)
            .Matches(@"^\+?[0-9]{7,15}$").WithMessage("Invalid phone number format")
            .When(x => x.Phone != null);
    }
}