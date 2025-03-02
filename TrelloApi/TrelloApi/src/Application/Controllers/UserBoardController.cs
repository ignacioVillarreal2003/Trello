using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TrelloApi.Application.Services.Interfaces;
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

    public UserBoardController(ILogger<UserBoardController> logger, IUserBoardService userBoardService)
    {
        _logger = logger;
        _userBoardService = userBoardService;
    }   
    
    [HttpGet("board/{boardId:int}")]
    public async Task<IActionResult> GetUsersByBoardId(int boardId)
    {
        try
        {
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
            UserBoardResponse userBoard = await _userBoardService.AddUserToBoard(boardId, dto);
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
            Boolean isDeleted = await _userBoardService.RemoveUserFromBoard(boardId, userId);
            if (!isDeleted)
            {
                _logger.LogDebug("User {UserId} not found in board {BoardId} for deletion.", userId, boardId);
                return NotFound(new { message = "User membership not found." });
            }

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