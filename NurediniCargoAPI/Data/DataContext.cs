using Microsoft.EntityFrameworkCore;
using NurediniCargoAPI.Entities;
using NurediniCargoAPI.EntityConfigurations;

namespace NurediniCargoAPI.Data
{
    public class DataContext(DbContextOptions<DataContext> opt) : DbContext(opt)
    {
        public DbSet<Goods> Goods { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<Warehouse> Warehouse { get; set; }

        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Movement> Movements { get; set; }
        public DbSet<GoodsSupplier> GoodsSupplier { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new GoodsConfiguration());
            modelBuilder.ApplyConfiguration(new SupplierConfiguration());
            modelBuilder.ApplyConfiguration(new WarehouseConfiguration());
            modelBuilder.ApplyConfiguration(new GoodsSupplierConfiguration());

            modelBuilder.ApplyConfiguration(new InventoryConfiguration());
            modelBuilder.ApplyConfiguration(new MovementConfiguration());
        }
    }
}
