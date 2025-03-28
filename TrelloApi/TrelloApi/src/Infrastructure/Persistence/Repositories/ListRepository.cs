using Microsoft.EntityFrameworkCore;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Generics;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Infrastructure.Persistence.Repositories;

public class ListRepository: GenericRepository<List>, IListRepository
{
    public ListRepository(IUnitOfWork unitOfWork): base(unitOfWork) { }
    
    public async Task<List> GetListByIdToAccessAsync(int listId)
    {
        return await Context.Lists
            .Include(l => l.Board)
            .FirstOrDefaultAsync(l => l.Id.Equals(listId));
    }
}