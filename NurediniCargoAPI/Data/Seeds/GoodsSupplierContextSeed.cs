using NurediniCargoAPI.Entities;

namespace NurediniCargoAPI.Data.Seeds
{
    public class GoodsSupplierContextSeed
    {
        public static async Task SeedAsync(DataContext dbContext)
        {
            if (!dbContext.GoodsSupplier.Any() && dbContext.Supplier.Any() && dbContext.Goods.Any())
            {

                var rand = new Random();
                List<Supplier> suppliers = [.. dbContext.Supplier];
                List<Goods> goods = [.. dbContext.Goods];

                for (int i = 0; i < 10; i++)
                {
                    // Pick a random warehouse and goods from the lists
                    var randomSupplier = suppliers[rand.Next(suppliers.Count)];
                    var randomGoods = goods[rand.Next(goods.Count)];

                    var existingRelationship = dbContext.GoodsSupplier
                        .FirstOrDefault(gs => gs.SupplierId == randomSupplier.Id && gs.GoodsId == randomGoods.Id);

                    if (existingRelationship == null)
                    {
                        // Create the GoodsSupplier relationship
                        var goodsSupplier = new GoodsSupplier
                        {
                            SupplierId = randomSupplier.Id,
                            GoodsId = randomGoods.Id,
                        };

                        await dbContext.GoodsSupplier.AddAsync(goodsSupplier);
                        await dbContext.SaveChangesAsync();
                    }

                }

                Console.WriteLine("Seed GoodsSupplier");
            }
        }
    }
}
