using TrelloApi.Domain.DTOs;

namespace TrelloApi.Domain.Interfaces.Services;

public interface IUserService
{
    Task<List<OutputUserDto>> GetUsers(int uid);
    Task<List<OutputUserDto>> GetUsersByUsername(string username, int uid);
    Task<List<OutputUserDto>> GetUsersByCardId(int taskId, int uid);
    Task<OutputUserDto?> RegisterUser(RegisterUserDto registerUserDto);
    Task<OutputUserDto?> LoginUser(LoginUserDto loginUserDto);
    Task<OutputUserDto?> UpdateUser(UpdateUserDto updateUserDto, int uid);
    Task<OutputUserDto?> DeleteUser(int uid);
}