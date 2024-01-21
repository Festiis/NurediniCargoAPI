using FluentValidation;
using NurediniCargoAPI.Entities;
using System;

namespace NurediniCargoAPI.Validators
{
    public class MovementValidator : AbstractValidator<Movement>
    {
        public MovementValidator()
        {
            RuleFor(m => m.GoodsId)
                .NotEmpty()
                .WithMessage("GoodsId is required.");

            RuleFor(m => m.DestinationWarehouseId)
                .NotEmpty()
                .WithMessage("DestinationWarehouseId is required.");

            RuleFor(m => m.Quantity)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0.");

            RuleFor(m => m.MovementDate)
                .NotEmpty()
                .WithMessage("MovementDate is required.");

            RuleFor(m => m.MovementBy)
                .NotEmpty()
                .WithMessage("MovementBy is required.");
        }
    }
}
