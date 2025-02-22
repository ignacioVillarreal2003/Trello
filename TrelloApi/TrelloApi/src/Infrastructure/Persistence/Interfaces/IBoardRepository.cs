using TrelloApi.Domain.Entities;

namespace TrelloApi.Infrastructure.Persistence.Interfaces;

public interface IBoardRepository: IGenericRepository<Board>
{
    Task<IEnumerable<Board>> GetBoardsByUserIdAsync(int userId);
}