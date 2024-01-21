using NurediniCargoAPI.Entities;

namespace NurediniCargoAPI.Repositories.Interfaces
{
    public interface IInventoryRepository : IAsyncRepository<Inventory>
    {
        Task<List<Warehouse?>> GetWarehouses(int goodsId);
    }
}