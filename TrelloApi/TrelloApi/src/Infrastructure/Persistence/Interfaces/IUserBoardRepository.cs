using TrelloApi.Domain.Entities;

namespace TrelloApi.Infrastructure.Persistence.Interfaces;

public interface IUserBoardRepository: IGenericRepository<UserBoard>
{
    Task<IEnumerable<User>> GetUsersByBoardIdAsync(int boardId);
}