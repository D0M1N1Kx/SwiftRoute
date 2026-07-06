using FluentValidation;
using RouteXY.Api.Modules.Warehouse.Requests;

namespace RouteXY.Api.Validators.Warehouses;

public class UpdateWarehouseRequestValidator : AbstractValidator<UpdateWarehouseRequest>
{
    public UpdateWarehouseRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100)
            .When(x => x.Name != null);
        
        RuleFor(x => x.City)
            .MaximumLength(100)
            .When(x => x.City != null);
        
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Invalid latitude")
            .When(x => x.Latitude != null);
        
        RuleFor(x => x.Longitude)
            .InclusiveBetween(-90, 90).WithMessage("Invalid longitude")
            .When(x => x.Longitude != null);
    }
}