using TrelloApi.Domain.Entities;

namespace TrelloApi.Domain.Interfaces.Repositories;

public interface IUserBoardRepository
{
    Task<UserBoard?> GetUserBoardById(int userId, int boardId);
    Task<List<User>> GetUsersForBoard(int boardId);
    Task<UserBoard?> AddUserBoard(UserBoard userBoard);
    Task<UserBoard?> DeleteUserBoard(UserBoard userBoard);
}