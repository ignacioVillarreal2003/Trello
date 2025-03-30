using Microsoft.EntityFrameworkCore;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Generics;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Infrastructure.Persistence.Repositories;

public class BoardRepository : GenericRepository<Board>, IBoardRepository
{
    public BoardRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

    public async Task<IEnumerable<Board>> GetBoardsByUserIdAsync(int userId)
    {
        return await Context.Boards
            .Join(Context.UserBoards, 
                board => board.Id,
                userBoard => userBoard.BoardId,
                (board, userBoard) => new { board, userBoard })
            .Where(ub => ub.userBoard.UserId == userId)
            .Select(ub => ub.board)
            .ToListAsync();
    }

    public async Task<Board> GetBoardByIdToAccessAsync(int boardId)
    {
        return await Context.Boards.FirstOrDefaultAsync(b=> b.Id.Equals(boardId));
    }

    public async Task<Board?> GetBoardByIdCompleteAsync(int boardId)
    {
        return await Context.Boards
            .Include(b => b.Lists)
            .ThenInclude(l => l.Cards)
            .FirstOrDefaultAsync(b => b.Id.Equals(boardId));
    }
}
