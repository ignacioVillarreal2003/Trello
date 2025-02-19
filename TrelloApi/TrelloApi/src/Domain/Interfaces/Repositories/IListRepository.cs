using TrelloApi.Domain.Entities;

namespace TrelloApi.Domain.Interfaces.Repositories;

public interface IListRepository
{
    Task<List?> GetListById(int listId);
    Task<List<List>> GetListsByBoardId(int boardId);
    Task AddList(List list);
    Task UpdateList(List list);
    Task DeleteList(List list);
}