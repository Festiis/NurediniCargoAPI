using NurediniCargoAPI.Entities;

namespace NurediniCargoAPI.Data.Seeds
{
    public class InventoryContextSeed
    {
        public static async Task SeedAsync(DataContext dbContext)
        {
            if (!dbContext.Inventory.Any())
            {
                dbContext.Inventory.AddRange(GetPreconfiguredInventory(dbContext.Warehouse.ToList(), dbContext.Goods.ToList()));
                await dbContext.SaveChangesAsync();
                Console.WriteLine("Seed Inventory");
            }
        }

        private static IEnumerable<Inventory> GetPreconfiguredInventory(List<Warehouse> warehouses, List<Goods> goods)
        {
            var rand = new Random();
            var inventoryList = new List<Inventory>();

            // Seed with a specific number of inventory items, adjust as needed
            for (int i = 0; i < 10; i++)
            {
                // Pick a random warehouse and goods from the lists
                var randomWarehouse = warehouses[rand.Next(warehouses.Count)];
                var randomGoods = goods[rand.Next(goods.Count)];

                // Create the inventory item with random warehouse, goods, and quantity
                var inventoryItem = new Inventory
                {
                    WarehouseId = randomWarehouse.Id,
                    GoodsId = randomGoods.Id,
                    Quantity = rand.Next(1, 51), // Random quantity between 1 and 50
                    CreatedBy = "SeedData",
                    CreatedDate = DateTime.Now,
                    LastModifiedBy = "SeedData",
                    LastModifiedDate = DateTime.Now
                };

                inventoryList.Add(inventoryItem);
            }

            return inventoryList;
        }
    }
}
