using TrelloApi.Domain.Entities;

namespace TrelloApi.Infrastructure.Persistence.Interfaces;

public interface IUserBoardRepository: IGenericRepository<UserBoard>
{
    Task<IEnumerable<User>> GetUsersByBoardIdAsync(int boardId);
    Task<UserBoard> GetUserBoardByIdAsync(int userId, int boardId);
    Task<UserBoard> GetUserBoardByIdToAccessAsync(int userId, int boardId);
}