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
    public class GoodsController(IAsyncRepository<Goods> repository, IValidator<Goods> validator) : ControllerBase, IBaseController<Goods>
    {
        private readonly IAsyncRepository<Goods> _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        private readonly IValidator<Goods> _validator = validator ?? throw new ArgumentNullException(nameof(validator));

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Goods>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Goods>>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return Ok(entities);
        }

        [HttpGet("{id}", Name = "GetGoods")]
        [ProducesResponseType(typeof(Goods), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Goods>> GetByIdAsync(int id)
        {
            var goods = await _repository.GetByIdAsync(id);
            if (goods == null)
            {
                return NotFound();
            }

            return Ok(goods);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Goods), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        public async Task<ActionResult<Goods>> AddAsync([FromBody] Goods goods)
        {
            try
            {
                _validator.ValidateAndThrow(goods);
                // Validation passed, continue processing

                var addedGoods = await _repository.AddAsync(goods);
                return CreatedAtRoute("GetGoods", new { id = addedGoods.Id }, addedGoods);
            }
            catch (ValidationException ex)
            {
                // Validation failed, handle the exception
                return UnprocessableEntity(ex.Errors); // Return validation errors in the response
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Goods), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Goods>> UpdateAsync(int id, [FromBody] Goods goods)
        {
            try
            {
                // Ensure the ID in the URL matches the ID in the request body
                if (id != goods.Id)
                {
                    return BadRequest("The ID in the URL does not match the ID in the request body.");
                }

                _validator.ValidateAndThrow(goods);

                // Check if the entity with the given ID exists in the repository
                var existingEntity = await _repository.GetByIdAsync(id);
                if (existingEntity == null)
                {
                    return NotFound("Entity not found.");
                }

                // Update the existing entity with the new data
                var updatedEntity = await _repository.UpdateAsync(goods);

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
            var goods = await _repository.GetByIdAsync(id);
            if (goods == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(goods);
            return NoContent();
        }
    }
}
