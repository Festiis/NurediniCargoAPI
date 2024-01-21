using Microsoft.AspNetCore.Mvc;
using NurediniCargoAPI.Controllers.Interfaces;
using NurediniCargoAPI.Entities;
using NurediniCargoAPI.Repositories.Interfaces;
using System.Net;

namespace NurediniCargoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodsSupplierController(IGoodsSupplierRepository repository) : ControllerBase, IGoodsSupplierController<GoodsSupplier>
    {
        private readonly IGoodsSupplierRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        [HttpPost("AssociateSupplierWithGoods")]
        [ProducesResponseType(typeof(GoodsSupplier), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<GoodsSupplier>> AssociateSupplierWithGoodsAsync(int supplierId, int goodsId)
        {
            if (supplierId == 0)
            {
                return BadRequest("No supplierId provided");
            }

            if (goodsId == 0)
            {
                return BadRequest("No goodsId provided");
            }

            return Ok(await _repository.AssociateSupplierWithGoodsAsync(supplierId, goodsId));
        }

        [HttpPost("DisassociateSupplierFromGoods")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DisassociateSupplierFromGoodsAsync(int supplierId, int goodsId)
        {
            if (supplierId == 0)
            {
                return BadRequest("No supplierId provided");
            }

            if (goodsId == 0)
            {
                return BadRequest("No goodsId provided");
            }

            if (await _repository.DisassociateSupplierFromGoodsAsync(supplierId, goodsId))
            {
                return Ok();
            }

            return NotFound();
        }

        [HttpGet("GetGoodsBySupplier/{supplierId}")]
        [ProducesResponseType(typeof(IEnumerable<Goods>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Goods>>> GetGoodsBySupplierAsync(int supplierId)
        {
            if (supplierId == 0)
            {
                return BadRequest("No supplierId provided");
            }

            return Ok(await _repository.GetGoodsBySupplierAsync(supplierId));
        }

        [HttpGet("GetSuppliersByGoods/{goodsId}")]
        [ProducesResponseType(typeof(IEnumerable<Supplier>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliersByGoodsAsync(int goodsId)
        {
            if (goodsId == 0)
            {
                return BadRequest("No goodsId provided");
            }

            return Ok(await _repository.GetSuppliersByGoodsAsync(goodsId));
        }
    }
}