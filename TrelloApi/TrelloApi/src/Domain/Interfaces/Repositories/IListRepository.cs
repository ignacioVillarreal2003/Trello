using TrelloApi.Domain.Entities.List;

namespace TrelloApi.Domain.Interfaces.Repositories;

public interface IListRepository
{
    Task<List?> GetListById(int listId);
    Task<List<List>> GetListsByBoardId(int boardId);
    Task<List?> UpdateList(List list);
    Task<List?> DeleteList(List list);
    Task<List?> AddList(List list);
}