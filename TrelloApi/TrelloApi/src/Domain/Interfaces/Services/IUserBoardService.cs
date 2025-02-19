using TrelloApi.Domain.DTOs;

namespace TrelloApi.Domain.Interfaces.Services;

public interface IUserBoardService
{
    Task<List<OutputUserDetailsDto>> GetUsersByBoardId(int boardId, int uid);
    Task<OutputUserBoardDetailsDto?> AddUserToBoard(int boardId, AddUserBoardDto dto, int uid);
    Task<Boolean> RemoveUserFromBoard(int boardId, int userId, int uid);
}