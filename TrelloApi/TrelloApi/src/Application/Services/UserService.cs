using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Application.Utils;
using TrelloApi.Domain.Constants;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.DTOs.User;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Application.Services;

public class UserService: BaseService, IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;
    private readonly IEncrypt _encrypt;
    
    public UserService(IMapper mapper, 
        IUnitOfWork unitOfWork,
        IUserRepository userRepository, 
        ILogger<UserService> logger, 
        IEncrypt encrypt)
        : base(mapper, unitOfWork)
    {
        _userRepository = userRepository;
        _logger = logger;
        _encrypt = encrypt;
    }

    public async Task<List<UserResponse>> GetUsers()
    {
        try
        {
            List<User> users = (await _userRepository.GetListAsync()).ToList();
            _logger.LogDebug("Retrieved {Count} users", users.Count);
            return _mapper.Map<List<UserResponse>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            throw;
        }
    }

    public async Task<List<UserResponse>> GetUsersByUsername(string username)
    {
        try
        {
            List<User> users = (await _userRepository.GetUsersByUsernameAsync(username)).ToList();
            _logger.LogDebug("Retrieved {Count} users {Username}", users.Count, username);
            return _mapper.Map<List<UserResponse>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users {Username}", username);
            throw;
        }
    }

    public async Task<List<UserResponse>> GetUsersByCardId(int cardId)
    {
        try
        {
            List<User> users = (await _userRepository.GetUsersByCardIdAsync(cardId)).ToList();
            _logger.LogDebug("Retrieved {Count} users for card {CardId}", users.Count, cardId);
            return _mapper.Map<List<UserResponse>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users for card {CardId}", cardId);
            throw;
        }
    }
    
    public async Task<UserResponse> RegisterUser(RegisterUserDto dto)
    {
        try
        {
            User user = new User(dto.Email, dto.Username, _encrypt.HashPassword(dto.Password), UserThemeValues.UserThemesAllowed[1]);
            await _userRepository.CreateAsync(user);

            await _unitOfWork.CommitAsync();
            _logger.LogInformation("User {Email} register", dto.Email);
            return _mapper.Map<UserResponse>(user);
        }
        catch (Exception ex)
        {
            _logger.LogInformation("User {Email} register", dto.Email);
            throw;
        }
    }

    public async Task<UserResponse?> LoginUser(LoginUserDto dto)
    {
        try
        {
            User? user = await _userRepository.GetAsync(u => u.Email.Equals(dto.Email));
            if (user == null)
            {
                _logger.LogError("Failed to login user {Email}", dto.Email);
                return null;
            }

            if (!_encrypt.ComparePassword(dto.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid user credentials.");
            }

            user.LastLogin = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("User {Email} login", dto.Email);
            return _mapper.Map<UserResponse>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error login user {Email}", dto.Email);
            throw;
        }
    }

    public async Task<UserResponse?> UpdateUser(UpdateUserDto dto, int userId)
    {
        try
        {
            User? user = await _userRepository.GetAsync(u => u.Id.Equals(userId));
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found for update", userId);
                return null;
            }
            
            if (!string.IsNullOrEmpty(dto.Username))
            {
                user.Username = dto.Username;
            }
            if (!string.IsNullOrEmpty(dto.Theme))
            {
                user.Theme = dto.Theme;
            }
            if (!string.IsNullOrEmpty(dto.OldPassword) && !string.IsNullOrEmpty(dto.NewPassword))
            {
                if (!_encrypt.ComparePassword(dto.OldPassword, user.Password))
                {
                    _logger.LogWarning("User {UserId} password not found for update", userId);
                    throw new UnauthorizedAccessException("Invalid user credentials.");
                }
                user.Password = _encrypt.HashPassword(dto.NewPassword);
            }

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("User {UserId} updated", userId);
            return _mapper.Map<UserResponse>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", userId);
            throw;
        }
    }

    public async Task<Boolean> DeleteUser(int userId)
    {
        try
        {
            User? user = await _userRepository.GetAsync(u => u.Id.Equals(userId));
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found for deletion", userId);
                return false;
            }

            await _userRepository.DeleteAsync(user);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("User {UserId} deleted", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", userId);
            throw;
        }
    }
}
