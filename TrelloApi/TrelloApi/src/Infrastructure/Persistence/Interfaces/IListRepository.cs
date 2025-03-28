using TrelloApi.Domain.Entities;

namespace TrelloApi.Infrastructure.Persistence.Interfaces;

public interface IListRepository: IGenericRepository<List>
{
    Task<List> GetListByIdToAccessAsync(int listId);

}