using NurediniCargoAPI.Entities;

namespace NurediniCargoAPI.Repositories.Interfaces
{
    public interface IGoodsSupplierRepository
    {
        Task<GoodsSupplier> AssociateSupplierWithGoodsAsync(int supplierId, int goodsId);
        Task<bool> DisassociateSupplierFromGoodsAsync(int supplierId, int goodsId);
        Task<IEnumerable<Goods?>> GetGoodsBySupplierAsync(int supplierId);
        Task<IEnumerable<Supplier?>> GetSuppliersByGoodsAsync(int goodsId);
    }
}
