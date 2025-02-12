using AutoMapper;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Application.Utils;
using TrelloApi.Domain.Entities.User;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;
using TrelloApi.Domain.User.DTO;

namespace TrelloApi.Application.Services;

public class UserService: BaseService, IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;
    private readonly IEncrypt _encrypt;
    
    public UserService(IMapper mapper, IBoardAuthorizationService boardAuthorizationService, IUserRepository userRepository, ILogger<UserService> logger, IEncrypt encrypt): base(mapper, boardAuthorizationService)
    {
        _userRepository = userRepository;
        _logger = logger;
        _encrypt = encrypt;
    }

    public async Task<List<OutputUserDto>> GetUsers(int userId)
    {
        try
        {
            List<User> users = await _userRepository.GetUsers();
            _logger.LogDebug("Retrieved {Count} users", users.Count);
            return _mapper.Map<List<OutputUserDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            throw;
        }
    }

    public async Task<List<OutputUserDto>> GetUsersByUsername(int userId, string username)
    {
        try
        {
            List<User> users = await _userRepository.GetUsersByUsername(username);
            _logger.LogDebug("Retrieved {Count} users {Username}", users.Count, username);
            return _mapper.Map<List<OutputUserDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users {Username}", username);
            throw;
        }
    }
    
    public async Task<List<OutputUserDto>> GetUsersByBoardId(int userId, int boardId)
    {
        try
        {
            List<User> users = await _userRepository.GetUsersByBoardId(boardId);
            _logger.LogDebug("Retrieved {Count} users for board {BoardId}", users.Count, boardId);
            return _mapper.Map<List<OutputUserDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users for board {BoardId}", boardId);
            throw;
        }
    }

    public async Task<List<OutputUserDto>> GetUsersByTaskId(int userId, int taskId)
    {
        try
        {
            List<User> users = await _userRepository.GetUsersByTaskId(taskId);
            _logger.LogDebug("Retrieved {Count} users for task {TaskId}", users.Count, taskId);
            return _mapper.Map<List<OutputUserDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users for task {TaskId}", taskId);
            throw;
        }
    }
    
    public async Task<OutputUserDto?> RegisterUser(RegisterUserDto registerUserDto)
    {
        try
        {
            User user = new User(registerUserDto.Email, registerUserDto.Username, _encrypt.HashPassword(registerUserDto.Password));
            User? newUser = await _userRepository.AddUser(user);
            if (newUser == null)
            {
                _logger.LogError("Failed to add user {Email}", registerUserDto.Email);
                return null;
            }

            _logger.LogInformation("User {Email} register", registerUserDto.Email);
            return _mapper.Map<OutputUserDto>(newUser);
        }
        catch (Exception ex)
        {
            _logger.LogInformation("User {Email} register", registerUserDto.Email);
            throw;
        }
    }

    public async Task<OutputUserDto?> LoginUser(LoginUserDto loginUserDto)
    {
        try
        {
            User? user = await _userRepository.GetUserByEmail(loginUserDto.Email);
            if (user == null)
            {
                _logger.LogError("Failed to login user {Email}", loginUserDto.Email);
                return null;
            }

            if (!_encrypt.ComparePassword(loginUserDto.Password, user.Password))
            {
                _logger.LogWarning("Invalid user credentials for user {Email}.", loginUserDto.Email);
                throw new UnauthorizedAccessException("Invalid user credentials.");
            }

            _logger.LogInformation("User {Email} login", loginUserDto.Email);
            return _mapper.Map<OutputUserDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error login user {Email}", loginUserDto.Email);
            throw;
        }
    }

    public async Task<OutputUserDto?> UpdateUser(int userId, UpdateUserDto updateUserDto)
    {
        try
        {
            User? user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found for update", userId);
                return null;
            }
            
            if (!string.IsNullOrEmpty(updateUserDto.Username))
            {
                user.Username = updateUserDto.Username;
            }
            if (!string.IsNullOrEmpty(updateUserDto.Theme))
            {
                user.Theme = updateUserDto.Theme;
            }
            if (!string.IsNullOrEmpty(updateUserDto.OldPassword) && !string.IsNullOrEmpty(updateUserDto.NewPassword))
            {
                if (!_encrypt.ComparePassword(updateUserDto.OldPassword, user.Password))
                {
                    _logger.LogWarning("User {UserId} password not found for update", userId);
                    throw new UnauthorizedAccessException("Invalid user credentials.");
                }
                user.Password = _encrypt.HashPassword(updateUserDto.NewPassword);
            }

            User? updatedUser = await _userRepository.UpdateUser(user);
            if (updatedUser == null)
            {
                _logger.LogError("Failed to update user {UserId}", userId);
                return null;
            }
            
            _logger.LogInformation("User {UserId} updated", userId);
            return _mapper.Map<OutputUserDto>(updatedUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", userId);
            throw;
        }
    }

    public async Task<OutputUserDto?> DeleteUser(int userId)
    {
        try
        {
            User? user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found for deletion", userId);
                return null;
            }

            User? deletedUser = await _userRepository.DeleteUser(user);
            if (deletedUser == null)
            {
                _logger.LogError("Failed to delete user {UserId}", userId);
                return null;
            }
            
            _logger.LogInformation("User {UserId} deleted", userId);
            return _mapper.Map<OutputUserDto>(deletedUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", userId);
            throw;
        }
    }
}
