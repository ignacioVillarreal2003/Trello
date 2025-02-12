namespace TrelloApi.Domain.Interfaces.Repositories;

public interface IUserBoardRepository
{
    Task<UserBoard.UserBoard?> GetUserBoardById(int userId, int boardId);
    Task<UserBoard.UserBoard?> AddUserBoard(UserBoard.UserBoard userBoard);
    Task<UserBoard.UserBoard?> DeleteUserBoard(UserBoard.UserBoard userBoard);
}