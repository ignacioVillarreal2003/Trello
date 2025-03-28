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
        try
        {
            BoardResponse? boardToAccess = await _boardService.GetBoardByIdToAccess(boardId);
            if (boardToAccess == null)
            {
                _logger.LogDebug("Board {BoardId} not found for got.", boardId);
                return NotFound(new { message = "Board not found." });
            }
        
            var authResult = await _authorizationService.AuthorizeAsync(User, boardToAccess.Id, "BoardAccessPolicy");
            if (!authResult.Succeeded)
            {
                return Forbid();
            }
            
            List<UserResponse> users = await _userBoardService.GetUsersByBoardId(boardId);
            _logger.LogDebug("Retrieved {Count} users for board {BoardId}", users.Count, boardId);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving board members for board {BoardId}", boardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    
    [HttpPost("board/{boardId:int}")]
    public async Task<IActionResult> AddUserToBoard(int boardId, [FromBody] AddUserBoardDto dto)
    {
        try
        {
            BoardResponse? boardToAccess = await _boardService.GetBoardByIdToAccess(boardId);
            if (boardToAccess == null)
            {
                _logger.LogDebug("Board {BoardId} not found for got.", boardId);
                return NotFound(new { message = "Board not found." });
            }
        
            var authResult = await _authorizationService.AuthorizeAsync(User, boardToAccess.Id, "BoardAccessPolicy");
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            UserBoardResponse userBoard = await _userBoardService.AddUserToBoard(boardId, dto);
            
            await _hubContext.Clients.Group(BoardId.ToString())
                .SendAsync("UserBoardCreated", userBoard);

            _logger.LogInformation("User {UserId} added to board {BoardId}", dto.UserId, boardId);
            return CreatedAtAction(nameof(GetUsersByBoardId), new { boardId = userBoard.BoardId }, userBoard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user {UserId} to board {BoardId}", dto.UserId, boardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }


    [HttpDelete("board/{boardId:int}/user/{userId:int}")]
    public async Task<IActionResult> RemoveUserFromBoard(int boardId, int userId)
    {
        try
        {
            BoardResponse? boardToAccess = await _boardService.GetBoardByIdToAccess(boardId);
            if (boardToAccess == null)
            {
                _logger.LogDebug("Board {BoardId} not found for got.", boardId);
                return NotFound(new { message = "Board not found." });
            }
        
            var authResult = await _authorizationService.AuthorizeAsync(User, boardToAccess.Id, "BoardAccessPolicy");
            if (!authResult.Succeeded)
            {
                return Forbid();
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user {UserId} from board {BoardId}.", userId, boardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}