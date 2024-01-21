using Microsoft.EntityFrameworkCore;
using NurediniCargoAPI.Data;
using NurediniCargoAPI.Entities;
using NurediniCargoAPI.Repositories.Interfaces;
using System.Linq.Expressions;

namespace NurediniCargoAPI.Repositories
{
    public class MovementRepository(DataContext dbContext) :  IMovementRepository
    {
        private readonly DataContext _dbContext = dbContext;

        public async Task<IEnumerable<Movement>> GetAllMovementsAsync()
        {
            return await _dbContext.Movements.ToListAsync();
        }

        public async Task<IEnumerable<Movement>> GetAllMovementsAsync(Expression<Func<Movement, bool>> filter)
        {
            return await _dbContext.Movements.Where(filter).ToListAsync();
        }

        public async Task<Movement?> GetMovementByIdAsync(int movementId)
        {
            return await _dbContext.Movements.FindAsync(movementId);
        }

        public async Task<IEnumerable<Movement>> GetMovementsByGoodsAsync(int goodsId)
        {
            return await GetAllMovementsAsync(m => m.GoodsId == goodsId);
        }

        public async Task<IEnumerable<Movement>> GetMovementsByWarehouseAsync(int warehouseId)
        {
            return await GetAllMovementsAsync(m => m.SourceWarehouseId == warehouseId || m.DestinationWarehouseId == warehouseId);
        }

        public async Task<IEnumerable<Movement>> GetMovementsBySupplierAsync(int supplierId)
        {
            return await GetAllMovementsAsync(m => m.SupplierId == supplierId);
        }

        public async Task<Movement> AddMovementAsync(Movement movement)
        {
            _dbContext.Movements.Add(movement);
            await _dbContext.SaveChangesAsync();
            return movement;
        }

        public async Task<Movement> UpdateMovementAsync(Movement movement)
        {
            _dbContext.Movements.Entry(movement).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return movement;
        }

        public async Task<bool> DeleteMovementAsync(Movement movement)
        {
            _dbContext.Movements.Remove(movement);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
