using AutoMapper;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.Constants;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.DTOs.User;
using TrelloApi.Domain.DTOs.UserBoard;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Application.Services;

public class UserBoardService: BaseService, IUserBoardService
{
    private readonly IUserBoardRepository _userBoardRepository;
    private readonly ILogger<UserBoardService> _logger;
    
    public UserBoardService(IMapper mapper, 
        IUnitOfWork unitOfWork,
        IUserBoardRepository userBoardRepository, 
        ILogger<UserBoardService> logger) 
        : base(mapper, unitOfWork)
    {
        _userBoardRepository = userBoardRepository;
        _logger = logger;
    } 
    
    public async Task<List<UserResponse>> GetUsersByBoardId(int boardId)
    {
        try
        {
            List<User> users = (await _userBoardRepository.GetUsersByBoardIdAsync(boardId)).ToList();
            _logger.LogDebug("Retrieved {Count} users for board {BoardId}", users.Count, boardId);
            return _mapper.Map<List<UserResponse>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users for board {BoardId}", boardId);
            throw;
        }
    }

    public async Task<UserBoardResponse> AddUserToBoard(int boardId, AddUserBoardDto dto)
    {
        try
        {
            UserBoard userBoard = new UserBoard(dto.UserId, boardId, dto.Role);
            await _userBoardRepository.CreateAsync(userBoard);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("User {UserId} added to board {BoardId}", dto.UserId, boardId);
            return _mapper.Map<UserBoardResponse>(userBoard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user {UserId} to board {BoardId}", dto.UserId, boardId);
            throw;
        }
    }

    public async Task<Boolean> RemoveUserFromBoard(int boardId, int userId)
    {
        try
        {
            UserBoard? userBoard = await _userBoardRepository.GetAsync(ub => ub.UserId.Equals(userId) && ub.BoardId.Equals(boardId));
            if (userBoard == null)
            {
                _logger.LogWarning("User {UserId} for board {BoardId} not found for deletion", userId, boardId);
                return false;
            }

            await _userBoardRepository.DeleteAsync(userBoard);
            await _unitOfWork.CommitAsync();

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