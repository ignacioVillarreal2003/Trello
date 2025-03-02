using TrelloApi.Domain.DTOs.Board;

namespace TrelloApi.Application.Services.Interfaces;

public interface IBoardService
{
    Task<BoardResponse?> GetBoardById(int boardId);
    Task<List<BoardResponse>> GetBoardsByUserId(int userId);
    Task<BoardResponse> AddBoard(AddBoardDto dto, int userId);
    Task<BoardResponse?> UpdateBoard(int boardId, UpdateBoardDto dto);
    Task<bool> DeleteBoard(int boardId);
}