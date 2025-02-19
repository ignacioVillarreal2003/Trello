using TrelloApi.Domain.Entities;

namespace TrelloApi.Domain.Interfaces.Repositories;

public interface IUserBoardRepository
{
    Task<UserBoard?> GetUserBoardById(int userId, int boardId);
    Task<List<User>> GetUsersByBoardId(int boardId);
    Task AddUserBoard(UserBoard userBoard);
    Task DeleteUserBoard(UserBoard userBoard);
}