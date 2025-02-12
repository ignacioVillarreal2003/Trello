using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.UserBoard;

namespace TrelloApi.Application.Services;

public class BoardAuthorizationService: IBoardAuthorizationService
{
    private readonly ILogger<BoardAuthorizationService> _logger;
    private readonly IUserBoardRepository _userBoardRepository;
    
    public BoardAuthorizationService(ILogger<BoardAuthorizationService> logger, IUserBoardRepository userBoardRepository)
    {
        _logger = logger;
        _userBoardRepository = userBoardRepository;
    }
    
    public async Task<Boolean> HasAccessToBoard(int userId, int boardId)
    {
        UserBoard? userBoard = await _userBoardRepository.GetUserBoardById(userId, boardId);
        if (userBoard == null)
        {
            _logger.LogWarning("User {UserId} attempted to access to board {BoardId} without permission.", userId, boardId);
            return false;
        }
        
        _logger.LogWarning("User {UserId} attempted to access to board {BoardId} successfully.", userId, boardId);
        return true;
    }
}