using TrelloApi.Domain.Entities.Board;

namespace TrelloApi.Domain.Interfaces.Services;

public interface IBoardService
{
    Task<OutputBoardDto?> GetBoardById(int boardId, int userId);
    Task<List<OutputBoardDto>> GetBoards(int userId);
    Task<OutputBoardDto?> AddBoard(AddBoardDto addBoardDto, int userId);
    Task<OutputBoardDto?> UpdateBoard(int boardId, UpdateBoardDto updateBoardDto, int userId);
    Task<OutputBoardDto?> DeleteBoard(int boardId, int userId);
}