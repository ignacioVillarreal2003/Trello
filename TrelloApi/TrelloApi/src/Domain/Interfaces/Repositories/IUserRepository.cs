using TrelloApi.Domain.Entities;

namespace TrelloApi.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserById(int userId);
    Task<User?> GetUserByEmail(string email);
    Task<List<User>> GetUsers();
    Task<List<User>> GetUsersByUsername(string username);
    Task<List<User>> GetUsersByCardId(int cardId);
    Task AddUser(User user);
    Task UpdateUser(User user);
    Task DeleteUser(User user);
}