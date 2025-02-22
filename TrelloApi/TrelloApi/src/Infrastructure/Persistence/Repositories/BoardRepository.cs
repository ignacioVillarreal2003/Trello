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
            .Where(ub => ub.userBoard.UserId == userId && ub.board.IsArchived == false)
            .Select(ub => ub.board)
            .ToListAsync();
    }
}
