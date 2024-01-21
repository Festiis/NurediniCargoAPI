using NurediniCargoAPI.Entities;

namespace NurediniCargoAPI.Data.Seeds
{
    public class SupplierContextSeed
    {
        public static async Task SeedAsync(DataContext dbContext)
        {
            if (!dbContext.Supplier.Any())
            {
                dbContext.Supplier.AddRange(GetPreconfiguredSupplier());
                await dbContext.SaveChangesAsync();
                Console.WriteLine("Seed Supplier");
            }
        }

        private static List<Supplier> GetPreconfiguredSupplier()
        {
            return
            [
                new Supplier
                {
                    Name = "SwedishTech Supplies",
                    ContactPerson = "Sven Andersson",
                    Email = "sven.andersson@swedishtech.se",
                    Phone = "+46701234567",
                    Address = "Storgatan 1",
                    City = "Stockholm",
                    PostalCode = "12345",
                    Country = "Sverige",
                    CreatedBy = "SeedData",
                    CreatedDate = DateTime.Now,
                    LastModifiedBy = "SeedData",
                    LastModifiedDate = DateTime.Now
                },
                new Supplier
                {
                    Name = "NordicCuisine Suppliers",
                    ContactPerson = "Eva Svensson",
                    Email = "eva.svensson@nordiccuisine.se",
                    Phone = "+46729876543",
                    Address = "Köksvägen 5",
                    City = "Göteborg",
                    PostalCode = "54321",
                    Country = "Sverige",
                    CreatedBy = "SeedData",
                    CreatedDate = DateTime.Now,
                    LastModifiedBy = "SeedData",
                    LastModifiedDate = DateTime.Now
                },
                new Supplier
                {
                    Name = "SwedishStyle Fashion",
                    ContactPerson = "Lars Lindström",
                    Email = "lars.lindstrom@swedishstyle.se",
                    Phone = "+46702345678",
                    Address = "Modegatan 10",
                    City = "Malmö",
                    PostalCode = "67890",
                    Country = "Sverige",
                    CreatedBy = "SeedData",
                    CreatedDate = DateTime.Now,
                    LastModifiedBy = "SeedData",
                    LastModifiedDate = DateTime.Now
                },
            ];
        }
    }
}
