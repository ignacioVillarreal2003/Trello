using TrelloApi.Domain.Entities;

namespace TrelloApi.Domain.Interfaces.Repositories;

public interface IBoardRepository
{
    Task<Board?> GetBoardById(int boardId);
    Task<List<Board>> GetBoardsByUserId(int userId);
    Task<Board?> AddBoard(Board board);
    Task<Board?> UpdateBoard(Board board);
    Task<Board?> DeleteBoard(Board board);
}