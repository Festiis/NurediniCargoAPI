using NurediniCargoAPI.Data;
using NurediniCargoAPI.Data.Seeds;
using NurediniCargoAPI.Extensions;

namespace NurediniCargoAPI
{
    public class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .MigrateDatabase<DataContext>((context) =>
                {
                    GoodsContextSeed.SeedAsync(context).Wait();
                    SupplierContextSeed.SeedAsync(context).Wait();
                    WarehouseContextSeed.SeedAsync(context).Wait();
                    InventoryContextSeed.SeedAsync(context).Wait();
                    GoodsSupplierContextSeed.SeedAsync(context).Wait();
                    MovementContextSeed.SeedAsync(context).Wait();
                })
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => 
            Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => 
            { 
                webBuilder.UseStartup<Startup>(); 
            });
    }
}