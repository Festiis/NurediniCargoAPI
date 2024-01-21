using Azure.Core;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NurediniCargoAPI.Controllers.Interfaces;
using NurediniCargoAPI.DTOs;
using NurediniCargoAPI.Entities;
using NurediniCargoAPI.Repositories;
using NurediniCargoAPI.Repositories.Interfaces;
using System.Linq.Expressions;
using System.Net;

namespace NurediniCargoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController(IInventoryRepository repository, IValidator<Inventory> validator) : ControllerBase
    {
        private readonly IInventoryRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        private readonly IValidator<Inventory> _validator = validator ?? throw new ArgumentNullException(nameof(validator));

        [HttpGet("Warehouses/{goodsId}")]
        [ProducesResponseType(typeof(IEnumerable<Warehouse?>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<Warehouse?>>> GetWarehouses(int goodsId)
        {
            var warehouses = await _repository.GetWarehouses(goodsId);

            if (warehouses == null || warehouses.Count == 0)
            {
                return NotFound($"Goods with ID {goodsId} not found in any warehouse.");
            }

            return Ok(warehouses);
        }

        [HttpPost("AddGoodsToWarehouse")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult?> AddGoodsToWarehouse([FromBody] InventoryDTO inventoryDTO) 
        {
            try
            {
                var inventory = new Inventory
                {
                    WarehouseId = inventoryDTO.WarehouseId,
                    GoodsId = inventoryDTO.GoodsId,
                    Quantity = inventoryDTO.Quantity,
                    CreatedBy = inventoryDTO.CreatedBy,
                    CreatedDate = inventoryDTO.CreatedDate,
                };

                _validator.ValidateAndThrow(inventory);
                // Validation passed, continue processing

                // Check if the goods already exist in the warehouse
                var existingInventory = await _repository.GetAsync(i => i.GoodsId == inventory.GoodsId && i.WarehouseId == inventory.WarehouseId);

                if (existingInventory.Any())
                {
                    // Goods already present, update the quantity
                    var inventoryEntry = existingInventory[0];
                    inventoryEntry.Quantity += inventoryDTO.Quantity;
                    inventoryEntry.LastModifiedBy = inventoryDTO.CreatedBy;
                    inventoryEntry.LastModifiedDate = inventoryDTO.CreatedDate;

                    await _repository.UpdateAsync(inventoryEntry);
                }
                else
                {
                    // Goods not present in the warehouse, create a new entry
                    inventory.LastModifiedBy = inventoryDTO.CreatedBy;
                    inventory.LastModifiedDate = inventoryDTO.CreatedDate;
                    await _repository.AddAsync(inventory);
                }

                return Ok("Goods added to warehouse successfully.");
            }
            catch (ValidationException ex)
            {
                // Validation failed, handle the exception
                return UnprocessableEntity(ex.Errors); // Return validation errors in the response
            }
        }

        [HttpDelete("RemoveGoodsFromWarehouse")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult?> RemoveGoodsFromWarehouse([FromBody] InventoryDTO inventoryDTO)
        {
            try
            {

                var inventory = new Inventory
                {
                    WarehouseId = inventoryDTO.WarehouseId,
                    GoodsId = inventoryDTO.GoodsId,
                    Quantity = inventoryDTO.Quantity,
                    CreatedBy = inventoryDTO.CreatedBy,
                    CreatedDate = inventoryDTO.CreatedDate,
                };

                _validator.ValidateAndThrow(inventory);
                // Validation passed, continue processing

                // Check if the goods exist in the warehouse

                var existingGoods = await _repository.GetAsync(i => i.GoodsId == inventory.GoodsId && i.WarehouseId == inventory.WarehouseId);

                if (!existingGoods.Any())
                {
                    // Goods not found in the warehouse
                    return NotFound("Goods not found in the warehouse.");
                }

                // Goods found, update the quantity
                var inventoryEntry = existingGoods[0];

                if (inventoryEntry.Quantity >= inventory.Quantity)
                {
                    // Sufficient quantity, update the quantity
                    inventoryEntry.Quantity -= inventory.Quantity;

                    // If the quantity becomes zero, you may want to delete the entry or handle it as needed
                    if (inventoryEntry.Quantity == 0)
                    {
                        await _repository.DeleteAsync(inventoryEntry);
                    }
                    else
                    {
                        inventoryEntry.LastModifiedBy = inventory.CreatedBy;
                        inventoryEntry.LastModifiedDate = inventory.CreatedDate;
                        await _repository.UpdateAsync(inventoryEntry);
                    }

                    return Ok("Goods removed from warehouse successfully.");
                }
                else
                {
                    // Insufficient quantity
                    return BadRequest("Insufficient quantity in the warehouse.");
                }
            }
            catch (ValidationException ex)
            {
                // Validation failed, handle the exception
                return UnprocessableEntity(ex.Errors); // Return validation errors in the response

            }
        }


        [HttpPost("MoveGoodsBetweenWarehouses")]
        public async Task<IActionResult?> MoveGoodsBetweenWarehouses([FromBody] MoveGoodsDTO moveGoodsDTO)
        {
            try
            {

                _validator.ValidateAndThrow(new Inventory {
                    WarehouseId = moveGoodsDTO.SourceWarehouseId,
                    GoodsId = moveGoodsDTO.GoodsId,
                    Quantity = moveGoodsDTO.Quantity,
                    CreatedBy = moveGoodsDTO.CreatedBy,
                    CreatedDate = moveGoodsDTO.CreatedDate,
                });

                _validator.ValidateAndThrow(new Inventory
                {
                    WarehouseId = moveGoodsDTO.DestinationWarehouseId,
                    GoodsId = moveGoodsDTO.GoodsId,
                    Quantity = moveGoodsDTO.Quantity,
                    CreatedBy = moveGoodsDTO.CreatedBy,
                    CreatedDate = moveGoodsDTO.CreatedDate,
                });
                // Validation passed, continue processing


                // Check if the goods exist in the source warehouse

                var sourceGoods = await _repository.GetAsync(i => i.GoodsId == moveGoodsDTO.GoodsId && i.WarehouseId == moveGoodsDTO.SourceWarehouseId);

                if (!sourceGoods.Any())
                {
                    // Goods not found in the source warehouse
                    return NotFound("Goods not found in the source warehouse.");
                }

                // Check if there is enough quantity to move
                var sourceInventoryEntry = sourceGoods[0];

                if (sourceInventoryEntry.Quantity >= moveGoodsDTO.Quantity)
                {
                    // There is enough quantity in the source warehouse, proceed with the move

                    // Check if the goods already exist in the destination warehouse

                    var destinationGoods = await _repository.GetAsync(i => i.GoodsId == moveGoodsDTO.GoodsId && i.WarehouseId == moveGoodsDTO.DestinationWarehouseId);

                    if (destinationGoods.Any())
                    {
                        // Goods already present in the destination warehouse, update the quantity
                        var destinationInventoryEntry = destinationGoods[0];
                        destinationInventoryEntry.Quantity += moveGoodsDTO.Quantity;
                        destinationInventoryEntry.LastModifiedBy = moveGoodsDTO.CreatedBy;
                        destinationInventoryEntry.LastModifiedDate = moveGoodsDTO.CreatedDate;
                        await _repository.UpdateAsync(destinationInventoryEntry);
                    }
                    else
                    {
                        // Goods not present in the destination warehouse, create a new entry
                        var newDestinationInventoryEntry = new Inventory
                        {
                            GoodsId = moveGoodsDTO.GoodsId,
                            WarehouseId = moveGoodsDTO.DestinationWarehouseId,
                            Quantity = moveGoodsDTO.Quantity,
                            CreatedBy = moveGoodsDTO.CreatedBy,
                            CreatedDate = moveGoodsDTO.CreatedDate,
                            LastModifiedBy = moveGoodsDTO.CreatedBy,
                            LastModifiedDate = moveGoodsDTO.CreatedDate,
                        };

                        await _repository.AddAsync(newDestinationInventoryEntry);
                    }

                    // Update the quantity in the source warehouse
                    sourceInventoryEntry.Quantity -= moveGoodsDTO.Quantity;

                    // If the quantity becomes zero, you may want to delete the entry or handle it as needed
                    if (sourceInventoryEntry.Quantity == 0)
                    {
                        await _repository.DeleteAsync(sourceInventoryEntry);
                    }
                    else
                    {
                        sourceInventoryEntry.LastModifiedBy = moveGoodsDTO.CreatedBy;
                        sourceInventoryEntry.LastModifiedDate = moveGoodsDTO.CreatedDate;
                        await _repository.UpdateAsync(sourceInventoryEntry);
                    }

                    return Ok("Goods moved between warehouses successfully.");
                }
                else
                {
                    // Insufficient quantity in the source warehouse
                    return BadRequest("Insufficient quantity in the source warehouse.");
                }
            }
            catch (ValidationException ex)
            {
                // Validation failed, handle the exception
                return UnprocessableEntity(ex.Errors); // Return validation errors in the response

            }

        }



        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Inventory?>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Inventory?>>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return Ok(entities);
        }

        [HttpGet("{id}", Name = "GetInventory")]
        [ProducesResponseType(typeof(Inventory), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Inventory?>> GetByIdAsync(int id)
        {
            var inventory = await _repository.GetByIdAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }

            return Ok(inventory);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Inventory), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Inventory?>> AddAsync([FromBody] InventoryDTO inventoryDTO)
        {
            try
            {
                var inventory = new Inventory
                {
                    WarehouseId = inventoryDTO.WarehouseId,
                    GoodsId = inventoryDTO.GoodsId,
                    Quantity = inventoryDTO.Quantity,
                    CreatedBy = inventoryDTO.CreatedBy,
                    CreatedDate = inventoryDTO.CreatedDate,
                    LastModifiedBy = inventoryDTO.LastModifiedBy,
                    LastModifiedDate = inventoryDTO.LastModifiedDate,    
                };

                _validator.ValidateAndThrow(inventory);
                // Validation passed, continue processing

                var addedInventory = await _repository.AddAsync(inventory);
                return CreatedAtRoute("GetInventory", new { id = addedInventory.Id }, addedInventory);
            }
            catch (ValidationException ex)
            {
                // Validation failed, handle the exception
                return UnprocessableEntity(ex.Errors); // Return validation errors in the response
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Inventory), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Inventory?>> UpdateAsync(int id, [FromBody] Inventory inventory)
        {
            try
            {
                // Ensure the ID in the URL matches the ID in the request body
                if (id != inventory.Id)
                {
                    return BadRequest("The ID in the URL does not match the ID in the request body.");
                }
                
                _validator.ValidateAndThrow(inventory);

                // Check if the entity with the given ID exists in the repository
                var existingEntity = await _repository.GetByIdAsync(id);
                if (existingEntity == null)
                {
                    return NotFound("Entity not found.");
                }

                // Update the existing entity with the new data
                var updatedEntity = await _repository.UpdateAsync(inventory);

                // Return the updated entity
                return Ok(updatedEntity);
            }
            catch (ValidationException ex)
            {
                // Validation failed, handle the exception
                return UnprocessableEntity(ex.Errors); // Return validation errors in the response
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult?> DeleteAsync(int id)
        {
            var inventory = await _repository.GetByIdAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(inventory);
            return NoContent();
        }
    }
}
