using FluentValidation;
using NurediniCargoAPI.Entities;

namespace NurediniCargoAPI.Validators
{
    public class GoodsValidator : AbstractValidator<Goods>
    {
        public GoodsValidator()
        {
            // Basic Goods Information
            RuleFor(goods => goods.Name)
                .MaximumLength(255).WithMessage("Name should not exceed 255 characters.")
                .NotEmpty().WithMessage("Please provide a name for the goods.");

            RuleFor(goods => goods.Description).MaximumLength(500).WithMessage("Description should not exceed 500 characters.");
            RuleFor(goods => goods.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");

            // Quantity and Availability
            RuleFor(goods => goods.MinimumOrderQuantity)
                .GreaterThanOrEqualTo(1).WithMessage("Minimum order quantity must be at least 1.");
            RuleFor(goods => goods.MaximumOrderQuantity)
                .GreaterThanOrEqualTo(goods => goods.MinimumOrderQuantity)
                .WithMessage("Maximum order quantity must be greater than or equal to the minimum order quantity.");

            // Logistics-related Attributes
            RuleFor(goods => goods.ShippingClass).MaximumLength(50).WithMessage("Shipping class should not exceed 50 characters.");
            RuleFor(goods => goods.HandlingInstructions).MaximumLength(200).WithMessage("Handling instructions should not exceed 200 characters.");

            // Additional attributes
            RuleFor(goods => goods.Brand).MaximumLength(50).WithMessage("Brand should not exceed 50 characters.");
            RuleFor(goods => goods.Category).MaximumLength(50).WithMessage("Category should not exceed 50 characters.");
            RuleFor(goods => goods.UnitOfMeasure).MaximumLength(20).WithMessage("Unit of measure should not exceed 20 characters.");
        }
    }
}