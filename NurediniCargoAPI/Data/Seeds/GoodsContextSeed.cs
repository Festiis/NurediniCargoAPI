using NurediniCargoAPI.Entities;

namespace NurediniCargoAPI.Data.Seeds
{
    public class GoodsContextSeed
    {
        public static async Task SeedAsync(DataContext dbContext)
        {
            if (!dbContext.Goods.Any())
            {
                dbContext.Goods.AddRange(GetPreconfiguredGoods());
                await dbContext.SaveChangesAsync();
                Console.WriteLine("Seed Goods");
            }
        }

        private static List<Goods> GetPreconfiguredGoods()
        {
            return
            [
                new Goods
                {
                    Name = "Laptop",
                    Description = "High-performance laptop with the latest technology.",
                    Price = 1200.00m,
                    MinimumOrderQuantity = 1,
                    MaximumOrderQuantity = 10,
                    ShippingClass = "Express",
                    HandlingInstructions = "Handle with care",
                    Brand = "XYZ",
                    Category = "Electronics",
                    UnitOfMeasure = "Piece",
                    CreatedBy = "SeedData",
                    CreatedDate = DateTime.Now,
                    LastModifiedBy = "SeedData",
                    LastModifiedDate = DateTime.Now
                },
                new Goods
                {
                    Name = "Smartphone",
                    Description = "Feature-packed smartphone with a large display.",
                    Price = 699.99m,
                    MinimumOrderQuantity = 1,
                    MaximumOrderQuantity = 5,
                    ShippingClass = "Standard",
                    HandlingInstructions = "Fragile",
                    Brand = "ABC",
                    Category = "Electronics",
                    UnitOfMeasure = "Piece",
                    CreatedBy = "SeedData",
                    CreatedDate = DateTime.Now,
                    LastModifiedBy = "SeedData",
                    LastModifiedDate = DateTime.Now
                },
                new Goods
                {
                    Name = "Running Shoes",
                    Description = "High-performance running shoes with cushioned soles.",
                    Price = 79.99m,
                    MinimumOrderQuantity = 1,
                    MaximumOrderQuantity = 10,
                    ShippingClass = "Standard",
                    HandlingInstructions = "Avoid exposure to direct sunlight",
                    Brand = "RunFlex",
                    Category = "Footwear",
                    UnitOfMeasure = "Pair",
                    CreatedBy = "SeedData",
                    CreatedDate = DateTime.Now,
                    LastModifiedBy = "SeedData",
                    LastModifiedDate = DateTime.Now
                },
            ];
        }
    }
}
