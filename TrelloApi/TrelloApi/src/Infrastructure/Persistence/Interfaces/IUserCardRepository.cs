using TrelloApi.Domain.Entities;

namespace TrelloApi.Infrastructure.Persistence.Interfaces;

public interface IUserCardRepository: IGenericRepository<UserCard>
{
    Task<IEnumerable<User>> GetUsersByCardIdAsync(int cardId);
}