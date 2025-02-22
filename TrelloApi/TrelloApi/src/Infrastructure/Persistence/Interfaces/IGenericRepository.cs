using System.Linq.Expressions;

namespace TrelloApi.Infrastructure.Persistence.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetAsync(Expression<Func<T, bool>> expression);

    Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>>? expression = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
    
    Task<T> CreateAsync(T entity);
    
    Task<T> UpdateAsync(T entity);

    Task<T> DeleteAsync(T entity);
}