using Microsoft.EntityFrameworkCore.Query;
using NurediniCargoAPI.Entities;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NurediniCargoAPI.Repositories.Interfaces
{
    public interface IAsyncRepository<T> where T : EntityBase
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);

        Task<T?> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
