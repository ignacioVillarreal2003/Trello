namespace TrelloApi.Domain.Interfaces.Repositories;

public interface IBoardRepository
{
    Task<Board.Board?> GetBoardById(int boardId);
    Task<List<Board.Board>> GetBoards(int userId);
    Task<Board.Board?> AddBoard(Board.Board board);
    Task<Board.Board?> UpdateBoard(Board.Board board);
    Task<Board.Board?> DeleteBoard(Board.Board board);
}