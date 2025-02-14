using TrelloApi.Domain.DTOs;

namespace TrelloApi.Domain.Interfaces.Services;

public interface IUserBoardService
{
    Task<List<OutputUserBoardListDto>> GetBoardMembers(int boardId, int userId);
    Task<OutputUserBoardDto?> AddMemberToBoard(int boardId, AddUserBoardDto dto, int uid);
    Task<bool> RemoveMemberFromBoard(int boardId, int userId, int uid);
}