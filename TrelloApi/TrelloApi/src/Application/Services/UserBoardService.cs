using AutoMapper;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;
using TrelloApi.Domain.UserBoard;
using TrelloApi.Domain.UserBoard.DTO;

namespace TrelloApi.Application.Services;

public class UserBoardService: BaseService, IUserBoardService
{
    private readonly IUserBoardRepository _userBoardRepository;
    private readonly ILogger<UserBoardService> _logger;

    public UserBoardService(IMapper mapper, IBoardAuthorizationService boardAuthorizationService, ILogger<UserBoardService> logger, IUserBoardRepository userBoardRepository): base(mapper, boardAuthorizationService)
    {
        _userBoardRepository = userBoardRepository;
        _logger = logger;
    }
    
    public async Task<OutputUserBoardDto?> GetUserBoardById(int userId, int boardId)
    {
        try
        {
            UserBoard? userBoard = await _userBoardRepository.GetUserBoardById(userId, boardId);
            if (userBoard == null)
            {
                _logger.LogWarning("User {UserId} for board {BoardId} not found.", userId, boardId);
                return null;
            }

            _logger.LogDebug("User {UserId} for board {BoardId} retrieved", userId, boardId);
            return _mapper.Map<OutputUserBoardDto>(userBoard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId} for board {BoardId}", userId, boardId);
            throw;
        }
    }
    
    public async Task<OutputUserBoardDto?> AddUserBoard(int userToAddId, int boardId, int userId)
    {
        try
        {
            UserBoard userBoard = new UserBoard(userToAddId, boardId);
            UserBoard? newUserBoard = await _userBoardRepository.AddUserBoard(userBoard);
            if (newUserBoard == null)
            {
                _logger.LogError("Failed to add user {UserId} to board {BoardId}", userToAddId, boardId);
                return null;
            }
            
            _logger.LogInformation("User {UserId} added to board {BoardId}", userToAddId, boardId);
            return _mapper.Map<OutputUserBoardDto>(newUserBoard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user {UserId} to board {BoardId}", userToAddId, boardId);
            throw;
        }
    }
    
    public async Task<OutputUserBoardDto?> DeleteUserBoard(int userToDeleteId, int boardId, int userId)
    {
        try
        {
            UserBoard? userBoard = await _userBoardRepository.GetUserBoardById(userToDeleteId, boardId);
            if (userBoard == null)
            {
                _logger.LogWarning("User {UserId} for board {BoardId} not found for deletion", userToDeleteId, boardId);
                return null;
            }

            UserBoard? deletedUserBoard = await _userBoardRepository.DeleteUserBoard(userBoard);
            if (deletedUserBoard == null)
            {
                _logger.LogError("Failed to delete user {UserId} for board {BoardId}", userToDeleteId, boardId);
                return null;
            }

            _logger.LogInformation("User {UserId} for board {BoardId} deleted", userToDeleteId, boardId);
            return _mapper.Map<OutputUserBoardDto>(deletedUserBoard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId} for board {BoardId}", userToDeleteId, boardId);
            throw;
        }
    }
}