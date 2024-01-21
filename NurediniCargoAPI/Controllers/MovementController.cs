using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NurediniCargoAPI.Entities;
using NurediniCargoAPI.Repositories;
using NurediniCargoAPI.Repositories.Interfaces;
using System.Net;

namespace NurediniCargoAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MovementController(IMovementRepository repository, IValidator<Movement> validator) : ControllerBase
    {
        private readonly IMovementRepository _repository = repository ?? throw new System.ArgumentNullException(nameof(repository));
        private readonly IValidator<Movement> _validator = validator ?? throw new ArgumentNullException(nameof(validator));

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<Movement>> GetAllMovementsAsync()
        {
            return Ok(await _repository.GetAllMovementsAsync());
        }

        [HttpGet("{id}", Name = "GetMovement")]
        [ProducesResponseType(typeof(GoodsSupplier), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Movement>> GetMovementByIdAsync(int id)
        {
            var movement = await _repository.GetMovementByIdAsync(id);
            if (movement == null)
            {
                return NotFound();
            }
            return Ok(movement);
        }

        [HttpGet("goods/{goodsId}")]
        [ProducesResponseType(typeof(IReadOnlyList<GoodsSupplier>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IReadOnlyList<Movement>>> GetMovementsByGoods(int goodsId)
        {
            return Ok(await _repository.GetMovementsByGoodsAsync(goodsId));
        }

        [HttpGet("warehouse/{warehouseId}")]
        [ProducesResponseType(typeof(IReadOnlyList<GoodsSupplier>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IReadOnlyList<Movement>>> GetMovementsByWarehouse(int warehouseId)
        {
            return Ok(await _repository.GetMovementsByWarehouseAsync(warehouseId));
        }

        [HttpGet("supplier/{supplierId}")]
        [ProducesResponseType(typeof(IReadOnlyList<GoodsSupplier>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IReadOnlyList<Movement>>> GetMovementsBySupplierAsync(int supplierId)
        {
            return Ok(await _repository.GetMovementsBySupplierAsync(supplierId));
        }

        [HttpPost]
        [ProducesResponseType(typeof(Goods), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<Movement>> AddMovementAsync([FromBody] Movement movement)
        {
            try
            {
                _validator.ValidateAndThrow(movement);
                // Validation passed, continue processing

                var addedMovement = await _repository.AddMovementAsync(movement);
                return CreatedAtRoute("GetMovement", new { id = addedMovement.Id }, addedMovement);
            }
            catch (ValidationException ex)
            {
                // Validation failed, handle the exception
                return UnprocessableEntity(ex.Errors); // Return validation errors in the response
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Movement), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Movement>> UpdateMovementAsync(int id, [FromBody] Movement movement)
        {
            try
            {
                // Ensure the ID in the URL matches the ID in the request body
                if (id != movement.Id)
                {
                    return BadRequest("The ID in the URL does not match the ID in the request body.");
                }

                // Check if the entity with the given ID exists in the repository
                var existingEntity = await _repository.GetMovementByIdAsync(id);
                if (existingEntity == null)
                {
                    return NotFound("Entity not found.");
                }

                _validator.ValidateAndThrow(movement);

                // Update the existing entity with the new data
                var updatedEntity = await _repository.UpdateMovementAsync(movement);

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
        public async Task<ActionResult> DeleteMovementAsync(int id)
        {
            var movement = await _repository.GetMovementByIdAsync(id);
            if (movement == null)
            {
                return NotFound();
            }

            await _repository.DeleteMovementAsync(movement);
            return NoContent();
        }
    }

}
