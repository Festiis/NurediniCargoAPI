using NurediniCargoAPI.Entities;

namespace NurediniCargoAPI.Data.Seeds
{
    public class WarehouseContextSeed
    {
        public static async Task SeedAsync(DataContext dbContext)
        {
            if (!dbContext.Warehouse.Any())
            {
                dbContext.Warehouse.AddRange(GetPreconfiguredWarehouse());
                await dbContext.SaveChangesAsync();
                Console.WriteLine("Seed Warehouse");
            }
        }

        private static List<Warehouse> GetPreconfiguredWarehouse()
        {
            return
            [
                new Warehouse
                {
                    Name = "Huvudlager",
                    Address = "Storgatan 1",
                    City = "Stockholm",
                    PostalCode = "12345",
                    Country = "Sweden",
                    CreatedBy = "SeedData",
                    CreatedDate = DateTime.Now,
                    LastModifiedBy = "SeedData",
                    LastModifiedDate = DateTime.Now
                },
                new Warehouse
                {
                    Name = "Norra Lager",
                    Address = "Nordvägen 5",
                    City = "Gothenburg",
                    PostalCode = "54321",
                    Country = "Sweden",
                    CreatedBy = "SeedData",
                    CreatedDate = DateTime.Now,
                    LastModifiedBy = "SeedData",
                    LastModifiedDate = DateTime.Now
                },
                new Warehouse
                {
                    Name = "Södra Lager",
                    Address = "Södra Gatan 10",
                    City = "Malmo",
                    PostalCode = "67890",
                    Country = "Sweden",
                    CreatedBy = "SeedData",
                    CreatedDate = DateTime.Now,
                    LastModifiedBy = "SeedData",
                    LastModifiedDate = DateTime.Now
                },
            ];
        }
    }
}
