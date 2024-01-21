using Microsoft.AspNetCore.Mvc;
using NurediniCargoAPI.Entities;

namespace NurediniCargoAPI.Controllers.Interfaces
{
    public interface IBaseController<T> where T : EntityBase
    {
        Task<ActionResult<IEnumerable<T>>> GetAllAsync();
        Task<ActionResult<T>> GetByIdAsync(int id);
        Task<ActionResult<T>> AddAsync([FromBody] T entity);
        Task<ActionResult<T>> UpdateAsync(int id, [FromBody] T entity);
        Task<ActionResult> DeleteAsync(int id);
    }
}
