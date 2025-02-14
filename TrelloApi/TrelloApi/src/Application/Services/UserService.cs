using AutoMapper;
using TrelloApi.Application.Utils;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;

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

    public async Task<List<OutputUserDto>> GetUsersByUsername(string username, int uid)
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

    public async Task<List<OutputUserDto>> GetUsersByCardId(int cardId, int uid)
    {
        try
        {
            List<User> users = await _userRepository.GetUsersByCardId(cardId);
            _logger.LogDebug("Retrieved {Count} users for card {CardId}", users.Count, cardId);
            return _mapper.Map<List<OutputUserDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users for card {CardId}", cardId);
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

    public async Task<OutputUserDto?> UpdateUser(UpdateUserDto updateUserDto, int uid)
    {
        try
        {
            User? user = await _userRepository.GetUserById(uid);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found for update", uid);
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
                    _logger.LogWarning("User {UserId} password not found for update", uid);
                    throw new UnauthorizedAccessException("Invalid user credentials.");
                }
                user.Password = _encrypt.HashPassword(updateUserDto.NewPassword);
            }

            User? updatedUser = await _userRepository.UpdateUser(user);
            if (updatedUser == null)
            {
                _logger.LogError("Failed to update user {UserId}", uid);
                return null;
            }
            
            _logger.LogInformation("User {UserId} updated", uid);
            return _mapper.Map<OutputUserDto>(updatedUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", uid);
            throw;
        }
    }

    public async Task<OutputUserDto?> DeleteUser(int uid)
    {
        try
        {
            User? user = await _userRepository.GetUserById(uid);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found for deletion", uid);
                return null;
            }

            User? deletedUser = await _userRepository.DeleteUser(user);
            if (deletedUser == null)
            {
                _logger.LogError("Failed to delete user {UserId}", uid);
                return null;
            }
            
            _logger.LogInformation("User {UserId} deleted", uid);
            return _mapper.Map<OutputUserDto>(deletedUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", uid);
            throw;
        }
    }
}
