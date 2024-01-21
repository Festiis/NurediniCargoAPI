using FluentValidation;
using NurediniCargoAPI.Entities;

namespace NurediniCargoAPI.Validators
{
    public class SupplierValidator : AbstractValidator<Supplier>
    {
        public SupplierValidator()
        {
            RuleFor(supplier => supplier.Name)
                .NotEmpty().WithMessage("Supplier name is required.")
                .MaximumLength(100).WithMessage("Name should not exceed 100 characters."); ;

            RuleFor(supplier => supplier.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(100).WithMessage("Address should not exceed 100 characters.");
            RuleFor(supplier => supplier.City).NotEmpty().WithMessage("City is required.").MaximumLength(50).WithMessage("City should not exceed 50 characters.");
            RuleFor(supplier => supplier.PostalCode).NotEmpty().WithMessage("Postal code is required.").MaximumLength(20).WithMessage("Postal code should not exceed 20 characters.");
            RuleFor(supplier => supplier.Country).MaximumLength(50).WithMessage("Country should not exceed 50 characters.");

            RuleFor(supplier => supplier.ContactPerson).MaximumLength(50).WithMessage("Contact person should not exceed 50 characters.");
            RuleFor(supplier => supplier.Email).EmailAddress().When(supplier => !string.IsNullOrEmpty(supplier.Email))
                .WithMessage("Invalid email format.");
            RuleFor(supplier => supplier.Phone).Matches(@"^\+?[0-9]+$").When(supplier => !string.IsNullOrEmpty(supplier.Phone))
                .WithMessage("Invalid phone number format. Use only digits, and you can include a leading '+'.");

        }
    }
}
