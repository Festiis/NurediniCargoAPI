using Microsoft.EntityFrameworkCore;
using NurediniCargoAPI.DTOs;
using NurediniCargoAPI.Entities;
using System.Linq.Expressions;

namespace NurediniCargoAPI.Data
{
    public class MovementContextSeed
    {
        public static async Task SeedAsync(DataContext dbContext)
        {
            if (!dbContext.Movements.Any())
            {
                List<Goods> goods = [.. dbContext.Goods];
                List<Warehouse> warehouses = [.. dbContext.Warehouse];
                List<Supplier> suppliers = [.. dbContext.Supplier];
                List<Inventory> inventory = [.. dbContext.Inventory];

                if (!(goods.Count != 0 && warehouses.Count != 0 && suppliers.Count != 0 && inventory.Count != 0))
                {
                    return;
                }


                var rand = new Random();

                // Seed with a specific number of movements, adjust as needed
                for (int i = 0; i < 10; i++)
                {
                    // Pick random goods, source warehouse, destination warehouse, and supplier from the lists
                    Goods randomGoods = goods[rand.Next(goods.Count)];
                    Warehouse randomSourceWarehouse = warehouses[rand.Next(warehouses.Count)];
                    Warehouse randomDestinationWarehouse = warehouses[rand.Next(warehouses.Count)];
                    Supplier randomSupplier = suppliers[rand.Next(suppliers.Count)];

                    var availableQuantityInSourceWarehouse = GetAvailableQuantityInWarehouse(inventory, randomGoods.Id, randomSourceWarehouse.Id);

                    // Create the movement item with random goods, warehouses, supplier, and quantity
                    Movement movementItem = new()
                    {
                        GoodsId = randomGoods.Id,
                        SourceWarehouseId = randomSourceWarehouse.Id,
                        DestinationWarehouseId = randomDestinationWarehouse.Id,
                        MovementDate = DateTime.Now,
                        MovementBy = "SeedData",
                        SupplierId = randomSupplier.Id,
                        // Generate a random quantity within the available quantity in the source warehouse
                        Quantity = rand.Next(1, availableQuantityInSourceWarehouse + 1)
                    };

                    // Add the movement item to the list
                    await dbContext.Movements.AddAsync(movementItem);
                    UpdateInventory(dbContext.Inventory, movementItem);
                    UpdateGoodsSupplier(dbContext.GoodsSupplier, movementItem);
                    await dbContext.SaveChangesAsync();
                }

                Console.WriteLine("Seed Movements");
            }
        }



        private static int GetAvailableQuantityInWarehouse(List<Inventory> inventory, int goodsId, int warehouseId)
        {
            var invotoryEntity = inventory.FirstOrDefault(i => i.WarehouseId == warehouseId && i.GoodsId == goodsId);

            if (invotoryEntity == null)
            {
                return 0;
            }

            return invotoryEntity.Quantity;
        }

        private static void UpdateInventory(DbSet<Inventory> inventories, Movement movement)
        {
            // Simulate updating the inventory based on the movement
            UpdateInventoryForMovement(inventories, movement.GoodsId, movement.SourceWarehouseId, -movement.Quantity);
            UpdateInventoryForMovement(inventories, movement.GoodsId, movement.DestinationWarehouseId, movement.Quantity);
        }

        private static void UpdateInventoryForMovement(DbSet<Inventory> inventories, int goodsId, int warehouseId, int quantityChange)
        {
            var inventoryEntry = inventories.FirstOrDefault(i => i.GoodsId == goodsId && i.WarehouseId == warehouseId);
            if (inventoryEntry == null)
            {
                if (quantityChange <= 0)
                {
                    return;
                }

                Inventory inventory = new()
                {
                    WarehouseId = warehouseId,
                    GoodsId = goodsId,
                    Quantity = quantityChange,
                    CreatedBy = "SeedData",
                    CreatedDate = DateTime.Now,
                    LastModifiedBy = "SeedData",
                    LastModifiedDate = DateTime.Now,
                };
                inventories.Add(inventory);
            }
            else
            {
                if (inventoryEntry.Quantity >= Math.Abs(quantityChange))
                {
                    // Sufficient quantity, update the quantity
                    inventoryEntry.Quantity -= quantityChange;

                    // If the quantity becomes zero, you may want to delete the entry or handle it as needed
                    if (inventoryEntry.Quantity == 0)
                    {
                        inventories.Remove(inventoryEntry);
                    }
                    else
                    {
                        inventoryEntry.LastModifiedBy = "SeedData";
                        inventoryEntry.LastModifiedDate = DateTime.Now;
                        inventories.Entry(inventoryEntry).State = EntityState.Modified;
                    }
                }
            }

        }

        private static void UpdateGoodsSupplier(DbSet<GoodsSupplier> goodsSuppliers, Movement movement)
        {
            var goodsSupplierEntry = goodsSuppliers.FirstOrDefault(gs => gs.SupplierId == movement.SupplierId && gs.GoodsId == movement.GoodsId);
            if (goodsSupplierEntry == null)
            {
                GoodsSupplier goodsSupplier = new()
                {
                    GoodsId = movement.GoodsId,
                    SupplierId = movement.SupplierId,
                };

                goodsSuppliers.Add(goodsSupplier);
            }
        }

    }
}
