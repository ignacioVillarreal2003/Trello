using TrelloApi.Domain.DTOs;

namespace TrelloApi.Domain.Interfaces.Services;

public interface IListService
{
    Task<OutputListDetailsDto?> GetListById(int listId, int uid);
    Task<List<OutputListDetailsDto>> GetListsByBoardId(int boardId, int uid);
    Task<OutputListDetailsDto?> AddList(int boardId, AddListDto dto, int uid);
    Task<OutputListDetailsDto?> UpdateList(int listId, UpdateListDto dto, int uid);
    Task<Boolean> DeleteList(int listId, int uid);
}