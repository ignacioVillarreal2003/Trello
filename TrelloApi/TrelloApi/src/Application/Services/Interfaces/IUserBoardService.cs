using TrelloApi.Domain.DTOs.User;
using TrelloApi.Domain.DTOs.UserBoard;

namespace TrelloApi.Application.Services.Interfaces;

public interface IUserBoardService
{
    Task<List<UserResponse>> GetUsersByBoardId(int boardId);
    Task<UserBoardResponse?> AddUserToBoard(int boardId, AddUserBoardDto dto);
    Task<Boolean> RemoveUserFromBoard(int boardId, int userId);
}