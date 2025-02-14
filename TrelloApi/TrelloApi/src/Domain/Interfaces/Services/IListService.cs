using TrelloApi.Domain.DTOs;

namespace TrelloApi.Domain.Interfaces.Services;

public interface IListService
{
    Task<OutputListDto?> GetListById(int listId, int uid);
    Task<List<OutputListDto>> GetListsByBoardId(int boardId, int uid);
    Task<OutputListDto?> UpdateList(int listId, UpdateListDto updateListDto, int uid);
    Task<OutputListDto?> DeleteList(int listId, int uid);
    Task<OutputListDto?> AddList(AddListDto addListDto, int boardId, int uid);
}