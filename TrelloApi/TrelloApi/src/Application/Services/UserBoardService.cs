using AutoMapper;
using TrelloApi.Application.Services.Interfaces;
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
        List<User> users = (await _userBoardRepository.GetUsersByBoardIdAsync(boardId)).ToList();
        _logger.LogDebug("Retrieved {Count} users for board {BoardId}", users.Count, boardId);
        return _mapper.Map<List<UserResponse>>(users);
    }

    public async Task<UserBoardResponse> AddUserToBoard(int boardId, AddUserBoardDto dto)
    {
        UserBoard newUserBoard = new UserBoard(dto.UserId, boardId);
        await _userBoardRepository.CreateAsync(newUserBoard);
        await _unitOfWork.CommitAsync();

        var userBoard = await _userBoardRepository
            .GetUserBoardByIdAsync(newUserBoard.UserId, newUserBoard.BoardId);
            
        _logger.LogInformation("User {UserId} added to board {BoardId}", dto.UserId, boardId);
        return _mapper.Map<UserBoardResponse>(userBoard);
    }

    public async Task<Boolean> RemoveUserFromBoard(int boardId, int userId)
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
}