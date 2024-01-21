using Microsoft.AspNetCore.Mvc;
using NurediniCargoAPI.Entities;

namespace NurediniCargoAPI.Controllers.Interfaces
{
    public interface IGoodsSupplierController<T> where T : GoodsSupplier
    {
        Task<ActionResult<T>> AssociateSupplierWithGoodsAsync(int supplierId, int goodsId);
        Task<IActionResult> DisassociateSupplierFromGoodsAsync(int supplierId, int goodsId);
        Task<ActionResult<IEnumerable<Goods>>> GetGoodsBySupplierAsync(int supplierId);
        Task<ActionResult<IEnumerable<Supplier>>> GetSuppliersByGoodsAsync(int goodsId);
    }
}