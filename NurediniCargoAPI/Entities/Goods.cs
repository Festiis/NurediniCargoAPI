using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NurediniCargoAPI.Entities
{
    public class Goods : EntityBase
    {

        // Basic Goods Information
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }


        // Quantity and Availability
        public int MinimumOrderQuantity { get; set; } // Minimum quantity that can be ordered
        public int MaximumOrderQuantity { get; set; } // Maximum quantity that can be ordered


        // Logistics-related Attributes
        public string? ShippingClass { get; set; } // e.g., Standard, Express
        public string? HandlingInstructions { get; set; }

        // Additional attributes
        public string? Brand { get; set; }
        public string? Category { get; set; }
        public string? UnitOfMeasure { get; set; }

        public ICollection<GoodsSupplier>? GoodsSupplier { get; set; }
    }
}
