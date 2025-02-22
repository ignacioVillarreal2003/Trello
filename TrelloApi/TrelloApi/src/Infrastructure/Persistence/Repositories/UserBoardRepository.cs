using Microsoft.EntityFrameworkCore;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Generics;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Infrastructure.Persistence.Repositories;

public class UserBoardRepository: GenericRepository<UserBoard>, IUserBoardRepository
{
    public UserBoardRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

    public async Task<IEnumerable<User>> GetUsersByBoardIdAsync(int boardId)
    {
        return await Context.Users
            .Join(Context.UserBoards, 
                user => user.Id, 
                userBoard => userBoard.UserId, 
                (user, userBoard) => new { user, userBoard })
            .Where(ub => ub.userBoard.BoardId.Equals(boardId))
            .Select(ub => ub.user)
            .ToListAsync();
    }
}