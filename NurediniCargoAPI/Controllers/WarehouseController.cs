using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NurediniCargoAPI.Controllers.Interfaces;
using NurediniCargoAPI.Entities;
using NurediniCargoAPI.Repositories.Interfaces;
using System.Net;

namespace NurediniCargoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController(IAsyncRepository<Warehouse> repository, IValidator<Warehouse> validator) : ControllerBase, IBaseController<Warehouse>
    {
        private readonly IAsyncRepository<Warehouse> _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        private readonly IValidator<Warehouse> _validator = validator ?? throw new ArgumentNullException(nameof(validator));

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Warehouse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Warehouse>>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return Ok(entities);
        }

        [HttpGet("{id}", Name = "GetWarehouse")]
        [ProducesResponseType(typeof(Warehouse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Warehouse>> GetByIdAsync(int id)
        {
            var warehouse = await _repository.GetByIdAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }

            return Ok(warehouse);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Warehouse), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<Warehouse>> AddAsync([FromBody] Warehouse warehouse)
        {
            try
            {
                _validator.ValidateAndThrow(warehouse);
                // Validation passed, continue processing

                var addedWarehouse = await _repository.AddAsync(warehouse);
                return CreatedAtRoute("GetWarehouse", new { id = addedWarehouse.Id }, addedWarehouse);
            }
            catch (ValidationException ex)
            {
                // Validation failed, handle the exception
                return UnprocessableEntity(ex.Errors); // Return validation errors in the response
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Warehouse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Warehouse>> UpdateAsync(int id, [FromBody] Warehouse warehouse)
        {
            try
            {
                // Ensure the ID in the URL matches the ID in the request body
                if (id != warehouse.Id)
                {
                    return BadRequest("The ID in the URL does not match the ID in the request body.");
                }

                _validator.ValidateAndThrow(warehouse);

                // Check if the entity with the given ID exists in the repository
                var existingEntity = await _repository.GetByIdAsync(id);
                if (existingEntity == null)
                {
                    return NotFound("Entity not found.");
                }

                // Update the existing entity with the new data
                var updatedEntity = await _repository.UpdateAsync(warehouse);

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
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var warehouse = await _repository.GetByIdAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(warehouse);
            return NoContent();
        }
    }
}
