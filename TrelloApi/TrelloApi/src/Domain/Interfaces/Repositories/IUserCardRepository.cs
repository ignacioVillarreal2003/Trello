using TrelloApi.Domain.Entities;

namespace TrelloApi.Domain.Interfaces.Repositories;

public interface IUserCardRepository
{
    Task<UserCard?> GetUserCardById(int userId, int cardId);
    Task<UserCard?> AddUserCard(UserCard userCard);
    Task<UserCard?> DeleteUserCard(UserCard userCard);
}