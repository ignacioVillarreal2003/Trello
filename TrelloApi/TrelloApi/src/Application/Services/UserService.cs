using AutoMapper;
using TrelloApi.Application.Utils;
using TrelloApi.Domain.Constants;
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

    public async Task<List<OutputUserDetailsDto>> GetUsers(int userId)
    {
        try
        {
            List<User> users = await _userRepository.GetUsers();
            _logger.LogDebug("Retrieved {Count} users", users.Count);
            return _mapper.Map<List<OutputUserDetailsDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            throw;
        }
    }

    public async Task<List<OutputUserDetailsDto>> GetUsersByUsername(string username, int uid)
    {
        try
        {
            List<User> users = await _userRepository.GetUsersByUsername(username);
            _logger.LogDebug("Retrieved {Count} users {Username}", users.Count, username);
            return _mapper.Map<List<OutputUserDetailsDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users {Username}", username);
            throw;
        }
    }

    public async Task<List<OutputUserDetailsDto>> GetUsersByCardId(int cardId, int uid)
    {
        try
        {
            List<User> users = await _userRepository.GetUsersByCardId(cardId);
            _logger.LogDebug("Retrieved {Count} users for card {CardId}", users.Count, cardId);
            return _mapper.Map<List<OutputUserDetailsDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users for card {CardId}", cardId);
            throw;
        }
    }
    
    public async Task<OutputUserDetailsDto?> RegisterUser(RegisterUserDto dto)
    {
        try
        {
            User user = new User(dto.Email, dto.Username, _encrypt.HashPassword(dto.Password), UserThemeValues.UserThemesAllowed[1]);
            await _userRepository.AddUser(user);

            _logger.LogInformation("User {Email} register", dto.Email);
            return _mapper.Map<OutputUserDetailsDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogInformation("User {Email} register", dto.Email);
            throw;
        }
    }

    public async Task<OutputUserDetailsDto?> LoginUser(LoginUserDto dto)
    {
        try
        {
            User? user = await _userRepository.GetUserByEmail(dto.Email);
            if (user == null)
            {
                _logger.LogError("Failed to login user {Email}", dto.Email);
                return null;
            }

            if (!_encrypt.ComparePassword(dto.Password, user.Password))
            {
                _logger.LogWarning("Invalid user credentials for user {Email}.", dto.Email);
                throw new UnauthorizedAccessException("Invalid user credentials.");
            }

            user.LastLogin = DateTime.UtcNow;
            await _userRepository.UpdateUser(user);
                
            _logger.LogInformation("User {Email} login", dto.Email);
            return _mapper.Map<OutputUserDetailsDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error login user {Email}", dto.Email);
            throw;
        }
    }

    public async Task<OutputUserDetailsDto?> UpdateUser(UpdateUserDto dto, int uid)
    {
        try
        {
            User? user = await _userRepository.GetUserById(uid);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found for update", uid);
                return null;
            }
            
            if (!string.IsNullOrEmpty(dto.Username))
            {
                user.Username = dto.Username;
            }
            if (!string.IsNullOrEmpty(dto.Theme) && UserThemeValues.UserThemesAllowed.Contains(dto.Theme))
            {
                user.Theme = dto.Theme;
            }
            if (!string.IsNullOrEmpty(dto.OldPassword) && !string.IsNullOrEmpty(dto.NewPassword))
            {
                if (!_encrypt.ComparePassword(dto.OldPassword, user.Password))
                {
                    _logger.LogWarning("User {UserId} password not found for update", uid);
                    throw new UnauthorizedAccessException("Invalid user credentials.");
                }
                user.Password = _encrypt.HashPassword(dto.NewPassword);
            }

            await _userRepository.UpdateUser(user);
            
            _logger.LogInformation("User {UserId} updated", uid);
            return _mapper.Map<OutputUserDetailsDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", uid);
            throw;
        }
    }

    public async Task<Boolean> DeleteUser(int uid)
    {
        try
        {
            User? user = await _userRepository.GetUserById(uid);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found for deletion", uid);
                return false;
            }

            await _userRepository.DeleteUser(user);
            
            _logger.LogInformation("User {UserId} deleted", uid);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", uid);
            throw;
        }
    }
}
