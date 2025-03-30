using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using TrelloApi.Application.Hub;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.Board;
using TrelloApi.Domain.DTOs.User;
using TrelloApi.Domain.DTOs.UserBoard;

namespace TrelloApi.Application.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
[EnableRateLimiting("fixed")]
public class UserBoardController: BaseController
{
    private readonly ILogger<UserBoardController> _logger;
    private readonly IUserBoardService _userBoardService;
    private readonly IHubContext<BoardHub> _hubContext;
    private readonly IBoardService _boardService;
    private readonly IAuthorizationService _authorizationService;
    
    public UserBoardController(ILogger<UserBoardController> logger, IUserBoardService userBoardService, IBoardService boardService, IHubContext<BoardHub> hubContext, IAuthorizationService authorizationService)
    {
        _logger = logger;
        _userBoardService = userBoardService;
        _hubContext = hubContext;
        _boardService = boardService;
        _authorizationService = authorizationService;
    }   
    
    [HttpGet("board/{boardId:int}")]
    public async Task<IActionResult> GetUsersByBoardId(int boardId)
    {
        var authorize = await TryAuthorizeBoardAsync(boardId);
        if (authorize != null)
        {
            return authorize;
        }
            
        List<UserResponse> users = await _userBoardService.GetUsersByBoardId(boardId);
        _logger.LogDebug("Retrieved {Count} users for board {BoardId}", users.Count, boardId);
        return Ok(users);
    }

    
    [HttpPost("board/{boardId:int}")]
    public async Task<IActionResult> AddUserToBoard(int boardId, [FromBody] AddUserBoardDto dto)
    {
        var authorize = await TryAuthorizeBoardAsync(boardId);
        if (authorize != null)
        {
            return authorize;
        }

        UserBoardResponse userBoard = await _userBoardService.AddUserToBoard(boardId, dto);
            
        await _hubContext.Clients.Group(BoardId.ToString())
            .SendAsync("UserBoardCreated", userBoard);

        _logger.LogInformation("User {UserId} added to board {BoardId}", dto.UserId, boardId);
        return CreatedAtAction(nameof(GetUsersByBoardId), new { boardId = userBoard.BoardId }, userBoard);
    }


    [HttpDelete("board/{boardId:int}/user/{userId:int}")]
    public async Task<IActionResult> RemoveUserFromBoard(int boardId, int userId)
    {
        var authorize = await TryAuthorizeBoardAsync(boardId);
        if (authorize != null)
        {
            return authorize;
        }

        Boolean isDeleted = await _userBoardService.RemoveUserFromBoard(boardId, userId);
        if (!isDeleted)
        {
            _logger.LogDebug("User {UserId} not found in board {BoardId} for deletion.", userId, boardId);
            return NotFound(new { message = "User membership not found." });
        }
            
        await _hubContext.Clients.Group(BoardId.ToString())
            .SendAsync("UserBoardDeleted", boardId, userId);

        _logger.LogInformation("User {UserId} removed from board {BoardId}.", userId, boardId);
        return NoContent();
    }
    
    private async Task<IActionResult?> TryAuthorizeBoardAsync (int boardId)
    {
        BoardResponse? board = await _boardService.GetBoardByIdToAccess(boardId);
        if (board == null)
        {
            _logger.LogDebug("Board {BoardId} not found for access.", boardId);
            return NotFound(new { message = "Board not found." });
        }

        var authResult = await _authorizationService.AuthorizeAsync(User, board.Id, "BoardAccessPolicy");
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        return null;
    }
}