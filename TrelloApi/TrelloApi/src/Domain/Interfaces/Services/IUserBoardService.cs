using TrelloApi.Domain.UserBoard.DTO;

namespace TrelloApi.Domain.Interfaces.Services;

public interface IUserBoardService
{
    Task<OutputUserBoardDto?> GetUserBoardById(int userId, int boardId);
    Task<OutputUserBoardDto?> AddUserBoard(int userToAddId, int boardId, int userId);
    Task<OutputUserBoardDto?> DeleteUserBoard(int userToDeleteId, int boardId, int userId);
}