using TrelloApi.Domain.DTOs.List;

namespace TrelloApi.Application.Services.Interfaces;

public interface IListService
{
    Task<ListResponse?> GetListById(int listId);
    Task<List<ListResponse>> GetListsByBoardId(int boardId);
    Task<ListResponse> AddList(int boardId, AddListDto dto);
    Task<ListResponse?> UpdateList(int listId, UpdateListDto dto);
    Task<Boolean> DeleteList(int listId);
}