using TrelloApi.app;
using TrelloApi.Domain.Board;
using Microsoft.EntityFrameworkCore;
using TrelloApi.Domain.Interfaces.Repositories;

namespace TrelloApi.Infrastructure.Persistence;

public class BoardRepository : Repository<Board>, IBoardRepository
{
    private readonly ILogger<BoardRepository> _logger;

    public BoardRepository(TrelloContext context, ILogger<BoardRepository> logger) : base(context)
    {
        _logger = logger;
    }

    public async Task<Board?> GetBoardById(int boardId)
    {
        try
        {
            Board? board = await Context.Boards
                .FirstOrDefaultAsync(b => b.Id.Equals(boardId));
            
            _logger.LogDebug("Board {BoardId} retrieval attempt completed", boardId);
            return board;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving board {BoardId}", boardId);
            throw;
        }
    }

    public async Task<List<Board>> GetBoards(int userId)
    {
        try
        {
            List<Board> boards = await Context.Boards
                .Join(Context.UserBoards, 
                    board => board.Id,
                    userBoard => userBoard.BoardId,
                    (board, userBoard) => new { board, userBoard })
                .Where(ub => ub.userBoard.UserId == userId)
                .Select(ub => ub.board)
                .ToListAsync();

            _logger.LogDebug("Retrieved {Count} boards for user {UserId}", boards.Count, userId);
            return boards;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving boards for user {UserId}", userId);
            throw;
        }
    }

    public async Task<Board?> AddBoard(Board board)
    {
        try
        {
            await Context.Boards.AddAsync(board);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("Board {BoardId} added successfully", board.Id);
            return board;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error adding board");
            throw;
        }
    }

    public async Task<Board?> UpdateBoard(Board board)
    {
        try
        {
            Context.Boards.Update(board);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("Board {BoardId} updated", board.Id);
            return board;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error updating board {BoardId}", board.Id);
            throw;
        }
    }

    public async Task<Board?> DeleteBoard(Board board)
    {
        try
        {
            Context.Boards.Remove(board);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("Board {BoardId} deleted", board.Id);
            return board;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error deleting board {BoardId}", board.Id);
            throw;
        }
    }
}
