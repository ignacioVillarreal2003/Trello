using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TrelloApi.Infrastructure.Persistence.Data;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Infrastructure.Persistence.Generics;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly IUnitOfWork _unitOfWork;
    protected TrelloContext Context => _unitOfWork.Context;

    protected GenericRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> expression)
    {
        var query = _unitOfWork.Context.Set<T>().AsQueryable();
        return await query.SingleOrDefaultAsync(expression);
    }

    public async Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>>? expression = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
    {
        IQueryable<T> query = _unitOfWork.Context.Set<T>();

        if (expression != null)
        {
            query = query.Where(expression);
        }

        if (orderBy != null)
        {
            return await orderBy(query).ToListAsync();
        }

        return await query.ToListAsync();
    }

    public async Task<T> CreateAsync(T entity)
    {
        await _unitOfWork.Context.Set<T>().AddAsync(entity);
        return entity;
    }

    public Task<T> UpdateAsync(T entity)
    {
        _unitOfWork.Context.Set<T>().Update(entity);
        return Task.FromResult(entity);
    }

    public Task<T> DeleteAsync(T entity)
    {
        _unitOfWork.Context.Set<T>().Remove(entity);
        return Task.FromResult(entity);
    }
}