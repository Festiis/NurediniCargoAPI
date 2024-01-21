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
    public class SupplierController(IAsyncRepository<Supplier> repository, IValidator<Supplier> validator) : ControllerBase, IBaseController<Supplier>
    {
        private readonly IAsyncRepository<Supplier> _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        private readonly IValidator<Supplier> _validator = validator ?? throw new ArgumentNullException(nameof(validator));

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Supplier>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return Ok(entities);
        }

        [HttpGet("{id}", Name = "GetSupplier")]
        [ProducesResponseType(typeof(Supplier), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Supplier>> GetByIdAsync(int id)
        {
            var supplier = await _repository.GetByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            return Ok(supplier);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Supplier), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<Supplier>> AddAsync([FromBody] Supplier supplier)
        {
            try
            {
                _validator.ValidateAndThrow(supplier);
                // Validation passed, continue processing

                var addedSupplier = await _repository.AddAsync(supplier);
                return CreatedAtRoute("GetSupplier", new { id = addedSupplier.Id }, addedSupplier);
            }
            catch (ValidationException ex)
            {
                // Validation failed, handle the exception
                return UnprocessableEntity(ex.Errors); // Return validation errors in the response
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Supplier), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Supplier>> UpdateAsync(int id, [FromBody] Supplier supplier)
        {
            try
            {
                // Ensure the ID in the URL matches the ID in the request body
                if (id != supplier.Id)
                {
                    return BadRequest("The ID in the URL does not match the ID in the request body.");
                }

                _validator.ValidateAndThrow(supplier);

                // Check if the entity with the given ID exists in the repository
                var existingEntity = await _repository.GetByIdAsync(id);
                if (existingEntity == null)
                {
                    return NotFound("Entity not found.");
                }

                // Update the existing entity with the new data
                var updatedEntity = await _repository.UpdateAsync(supplier);

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
            var supplier = await _repository.GetByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(supplier);
            return NoContent();
        }
    }
}
