namespace TrelloApi.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<Entities.User.User?> GetUserById(int userId);
    Task<Entities.User.User?> GetUserByEmail(string email);
    Task<List<Entities.User.User>> GetUsers();
    Task<List<Entities.User.User>> GetUsersByUsername(string username);
    Task<List<Entities.User.User>> GetUsersByBoardId(int boardId);
    Task<List<Entities.User.User>> GetUsersByTaskId(int taskId);
    Task<Entities.User.User?> AddUser(Entities.User.User user);
    Task<Entities.User.User?> UpdateUser(Entities.User.User user);
    Task<Entities.User.User?> DeleteUser(Entities.User.User user);
}