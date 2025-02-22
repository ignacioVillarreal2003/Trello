using TrelloApi.Domain.Entities;

namespace TrelloApi.Infrastructure.Persistence.Interfaces;

public interface IUserRepository: IGenericRepository<User>
{
    Task<IEnumerable<User>> GetUsersByUsernameAsync(string username);
    Task<IEnumerable<User>> GetUsersByCardIdAsync(int cardId);
}