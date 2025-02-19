using TrelloApi.app;
using Microsoft.EntityFrameworkCore;
using TrelloApi.Domain.Entities;
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
        Board? board = await Context.Boards
            .FirstOrDefaultAsync(b => b.Id.Equals(boardId) && b.IsArchived.Equals(false));
            
        _logger.LogDebug("Board {BoardId} retrieval attempt completed", boardId);
        return board;
    }

    public async Task<List<Board>> GetBoardsByUserId(int userId)
    {
        List<Board> boards = await Context.Boards
            .Join(Context.UserBoards, 
                board => board.Id,
                userBoard => userBoard.BoardId,
                (board, userBoard) => new { board, userBoard })
            .Where(ub => ub.userBoard.UserId == userId && ub.board.IsArchived == false)
            .Select(ub => ub.board)
            .ToListAsync();

        _logger.LogDebug("Retrieved {Count} boards for user {UserId}", boards.Count, userId);
        return boards;
    }

    public async Task<Board> AddBoard(Board board)
    {
        await Context.Boards.AddAsync(board);
        await Context.SaveChangesAsync();
        _logger.LogDebug("Board {BoardId} added successfully", board.Id);
        return board;
    }

    public async Task<Board> UpdateBoard(Board board)
    {
        Context.Boards.Update(board);
        await Context.SaveChangesAsync();
        _logger.LogDebug("Board {BoardId} updated", board.Id);
        return board;
    }

    public async Task<Board> DeleteBoard(Board board)
    {
        Context.Boards.Remove(board);
        await Context.SaveChangesAsync();
        _logger.LogDebug("Board {BoardId} deleted", board.Id);
        return board;
    }
}
