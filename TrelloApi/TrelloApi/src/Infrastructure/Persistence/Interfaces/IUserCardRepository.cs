using TrelloApi.Domain.Entities;

namespace TrelloApi.Infrastructure.Persistence.Interfaces;

public interface IUserCardRepository: IGenericRepository<UserCard>
{
    Task<IEnumerable<User>> GetUsersByCardIdAsync(int cardId);
    Task<UserCard> GetUserCardByIdAsync(int userId, int cardId);
    Task<UserCard> GetUserCardByIdToAccessAsync(int userId, int cardId);
}