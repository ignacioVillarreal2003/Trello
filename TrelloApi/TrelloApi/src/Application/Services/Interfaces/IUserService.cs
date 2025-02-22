using TrelloApi.Domain.DTOs.User;

namespace TrelloApi.Application.Services.Interfaces;

public interface IUserService
{
    Task<List<UserResponse>> GetUsers();
    Task<List<UserResponse>> GetUsersByUsername(string username);
    Task<List<UserResponse>> GetUsersByCardId(int taskId);
    Task<UserResponse?> RegisterUser(RegisterUserDto dto);
    Task<UserResponse?> LoginUser(LoginUserDto dto);
    Task<UserResponse?> UpdateUser(UpdateUserDto dto, int userId);
    Task<Boolean> DeleteUser(int userId);
}