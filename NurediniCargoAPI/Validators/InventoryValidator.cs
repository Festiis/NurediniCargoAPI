using FluentValidation;
using NurediniCargoAPI.Entities;

namespace NurediniCargoAPI.Validators
{
    public class InventoryValidator : AbstractValidator<Inventory>
    {
        public InventoryValidator()
        {
            RuleFor(inventory => inventory.WarehouseId).NotEmpty().WithMessage("WarehouseId is required.");
            RuleFor(inventory => inventory.GoodsId).NotEmpty().WithMessage("GoodsId is required.");
            RuleFor(inventory => inventory.Quantity)
                .NotEmpty().WithMessage("Quantity is required.")
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        }
    }
}
