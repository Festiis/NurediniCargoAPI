using Microsoft.EntityFrameworkCore;
using NurediniCargoAPI.Data;
using NurediniCargoAPI.Entities;
using NurediniCargoAPI.Repositories.Interfaces;

namespace NurediniCargoAPI.Repositories
{
    public class InventoryRepository(DataContext dbContext) : RepositoryBase<Inventory>(dbContext), IInventoryRepository
    {
        public async Task<List<Warehouse?>> GetWarehouses(int GoodsId)
        {
            return await _dbContext.Inventory
                .Where(i => i.GoodsId == GoodsId)
                .Select(i => i.Warehouse)
                .ToListAsync();
        }

    }
}
