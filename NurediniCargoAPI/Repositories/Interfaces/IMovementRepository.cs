using NurediniCargoAPI.Entities;
using System.Linq.Expressions;

namespace NurediniCargoAPI.Repositories.Interfaces
{
    public interface IMovementRepository
    {
        Task<IEnumerable<Movement>> GetAllMovementsAsync();
        Task<IEnumerable<Movement>> GetAllMovementsAsync(Expression<Func<Movement, bool>> filter);

        Task<Movement?> GetMovementByIdAsync(int movementId);
        Task<IEnumerable<Movement>> GetMovementsByGoodsAsync(int goodsId);
        Task<IEnumerable<Movement>> GetMovementsByWarehouseAsync(int warehouseId);
        Task<IEnumerable<Movement>> GetMovementsBySupplierAsync(int supplierId);
        Task<Movement> AddMovementAsync(Movement movement);
        Task<Movement> UpdateMovementAsync(Movement movement);
        Task<bool> DeleteMovementAsync(Movement movement);
    }

}
