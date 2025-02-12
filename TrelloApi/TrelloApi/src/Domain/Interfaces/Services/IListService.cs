using TrelloApi.Domain.Entities.List;

namespace TrelloApi.Domain.Interfaces.Services;

public interface IListService
{
    Task<OutputListDto?> GetListById(int listId, int userId);
    Task<List<OutputListDto>> GetListsByBoardId(int boardId, int userId);
    Task<OutputListDto?> UpdateList(int listId, UpdateListDto updateListDto, int userId);
    Task<OutputListDto?> DeleteList(int listId, int userId);
    Task<OutputListDto?> AddList(AddListDto addListDto, int boardId, int userId);
}