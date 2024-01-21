using FluentValidation;
using NurediniCargoAPI.Entities;

namespace NurediniCargoAPI.Validators
{
    public class WarehouseValidator : AbstractValidator<Warehouse>
    {
        public WarehouseValidator()
        {
            RuleFor(warehouse => warehouse.Name)
                .NotEmpty()
                .WithMessage("Warehouse name is required.")
                .MaximumLength(100)
                .WithMessage("Name should not exceed 100 characters.");

            RuleFor(warehouse => warehouse.Address).MaximumLength(100).WithMessage("Address should not exceed 100 characters.");
            RuleFor(warehouse => warehouse.City).MaximumLength(50).WithMessage("City should not exceed 50 characters.");
            RuleFor(warehouse => warehouse.PostalCode).MaximumLength(20).WithMessage("Postal code should not exceed 20 characters.");
            RuleFor(warehouse => warehouse.Country).MaximumLength(50).WithMessage("Country should not exceed 50 characters.");
        }
    }
}
