using TrelloApi.Domain.Entities.User;
using TrelloApi.Domain.User.DTO;

namespace TrelloApi.Domain.Interfaces.Services;

public interface IUserService
{
    Task<List<OutputUserDto>> GetUsers(int userId);
    Task<List<OutputUserDto>> GetUsersByUsername(int userId, string username);
    Task<List<OutputUserDto>> GetUsersByBoardId(int userId, int boardId);
    Task<List<OutputUserDto>> GetUsersByTaskId(int userId, int taskId);
    Task<OutputUserDto?> RegisterUser(RegisterUserDto registerUserDto);
    Task<OutputUserDto?> LoginUser(LoginUserDto loginUserDto);

    Task<OutputUserDto?> UpdateUser(int userId, UpdateUserDto updateUserDto);
    Task<OutputUserDto?> DeleteUser(int userId);
}