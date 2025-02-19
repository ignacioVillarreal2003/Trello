using TrelloApi.Domain.DTOs;

namespace TrelloApi.Domain.Interfaces.Services;

public interface IUserService
{
    Task<List<OutputUserDetailsDto>> GetUsers(int uid);
    Task<List<OutputUserDetailsDto>> GetUsersByUsername(string username, int uid);
    Task<List<OutputUserDetailsDto>> GetUsersByCardId(int taskId, int uid);
    Task<OutputUserDetailsDto?> RegisterUser(RegisterUserDto dto);
    Task<OutputUserDetailsDto?> LoginUser(LoginUserDto dto);
    Task<OutputUserDetailsDto?> UpdateUser(UpdateUserDto dto, int uid);
    Task<Boolean> DeleteUser(int uid);
}