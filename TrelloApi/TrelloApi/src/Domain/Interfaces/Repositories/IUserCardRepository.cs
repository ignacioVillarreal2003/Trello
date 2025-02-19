using TrelloApi.Domain.Entities;

namespace TrelloApi.Domain.Interfaces.Repositories;

public interface IUserCardRepository
{
    Task<UserCard?> GetUserCardById(int userId, int cardId);
    Task<List<User>> GetUsersByCardId(int cardId);
    Task AddUserCard(UserCard userCard);
    Task DeleteUserCard(UserCard userCard);
}