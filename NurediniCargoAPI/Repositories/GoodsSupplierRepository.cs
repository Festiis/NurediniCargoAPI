using Microsoft.EntityFrameworkCore;
using NurediniCargoAPI.Data;
using NurediniCargoAPI.Entities;
using NurediniCargoAPI.Repositories.Interfaces;

namespace NurediniCargoAPI.Repositories
{
    public class GoodsSupplierRepository(DataContext context) : IGoodsSupplierRepository
    {
        protected readonly DataContext _dbContext = context;

        public async Task<GoodsSupplier> AssociateSupplierWithGoodsAsync(int supplierId, int goodsId)
        {
            var existingAssociation = await _dbContext.GoodsSupplier
                .Where(gs => gs.SupplierId == supplierId && gs.GoodsId == goodsId)
                .FirstOrDefaultAsync();

            if (existingAssociation != null)
            {
                return existingAssociation;
            }

            // If not, create a new association
            var newAssociation = new GoodsSupplier { SupplierId = supplierId, GoodsId = goodsId };

            await _dbContext.GoodsSupplier.AddAsync(newAssociation);
            await _dbContext.SaveChangesAsync();
            return newAssociation;
        }

        public async Task<bool> DisassociateSupplierFromGoodsAsync(int supplierId, int goodsId)
        {
            // Find and remove the association
            var associationToRemove = await _dbContext.GoodsSupplier
                .Where(gs => gs.SupplierId == supplierId && gs.GoodsId == goodsId)
                .FirstOrDefaultAsync();

            if (associationToRemove != null)
            {
                _dbContext.GoodsSupplier.Remove(associationToRemove);

                return await _dbContext.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<IEnumerable<Goods?>> GetGoodsBySupplierAsync(int supplierId)
        {
            return await _dbContext.GoodsSupplier
                .Where(gs => gs.SupplierId == supplierId)
                .Include(gs => gs.Goods)
                .Select(gs => gs.Goods)
                .ToListAsync();
        }

        public async Task<IEnumerable<Supplier?>> GetSuppliersByGoodsAsync(int goodsId)
        {
            return await _dbContext.GoodsSupplier
                .Where(gs => gs.GoodsId == goodsId)
                .Include(gs => gs.Supplier)
                .Select(gs => gs.Supplier)
                .ToListAsync();
        }
    }
}
