using AutoMapper;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Application.Services;

public class UserBoardService: BaseService, IUserBoardService
{
    private readonly IUserBoardRepository _userBoardRepository;
    private readonly ILogger<UserBoardService> _logger;
    
    public UserBoardService(IMapper mapper, IBoardAuthorizationService boardAuthorizationService, IUserBoardRepository userBoardRepository, ILogger<UserBoardService> logger) : base(mapper, boardAuthorizationService)
    {
        _userBoardRepository = userBoardRepository;
        _logger = logger;
    } 
    
        public async Task<List<OutputUserBoardListDto>> GetBoardMembers(int boardId, int uid)
    {
        try
        {
            List<User> users = await _userBoardRepository.GetUsersForBoard(boardId);
            _logger.LogDebug("Retrieved {Count} users for board {BoardId}", users.Count, boardId);
            return _mapper.Map<List<OutputUserBoardListDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users for board {BoardId}", boardId);
            throw;
        }
    }

    public async Task<OutputUserBoardDto?> AddMemberToBoard(int boardId, AddUserBoardDto dto, int uid)
    {
        try
        {
            UserBoard userBoard = new UserBoard(dto.UserId, boardId);
            UserBoard? newUserBoard = await _userBoardRepository.AddUserBoard(userBoard);
            if (newUserBoard == null)
            {
                _logger.LogError("Failed to add user {UserId} to board {BoardId}", dto.UserId, boardId);
                return null;
            }
            
            _logger.LogInformation("User {UserId} added to board {BoardId}", dto.UserId, boardId);
            return _mapper.Map<OutputUserBoardDto>(newUserBoard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user {UserId} to board {BoardId}", dto.UserId, boardId);
            throw;
        }
    }

    public async Task<bool> RemoveMemberFromBoard(int boardId, int userId, int uid)
    {
        try
        {
            UserBoard? userBoard = await _userBoardRepository.GetUserBoardById(userId, boardId);
            if (userBoard == null)
            {
                _logger.LogWarning("User {UserId} for board {BoardId} not found for deletion", userId, boardId);
                return false;
            }

            UserBoard? deletedUserBoard = await _userBoardRepository.DeleteUserBoard(userBoard);
            if (deletedUserBoard == null)
            {
                _logger.LogError("Failed to delete user {UserId} for board {BoardId}", userId, boardId);
                return false;
            }

            _logger.LogInformation("User {UserId} for board {BoardId} deleted", userId, boardId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId} for board {BoardId}", userId, boardId);
            throw;
        }
    }
}