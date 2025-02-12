using Microsoft.AspNetCore.Mvc;
using TrelloApi.Application.Filters;
using TrelloApi.Application.Services;
using TrelloApi.Domain.Interfaces.Services;
using TrelloApi.Domain.UserBoard;
using TrelloApi.Domain.UserBoard.DTO;

namespace TrelloApi.Application.Controllers;

[ApiController]
[Route("[controller]")]
[RequireAuthentication]
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
    public async Task<IActionResult> GetUserBoardById(int boardId)
    {
        try
        {
            OutputUserBoardDto? userBoard = await _userBoardService.GetUserBoardById(UserId, boardId);
            if (userBoard == null)
            {
                _logger.LogDebug("User {UserId} to board {BoardId} not found", UserId, boardId);
                return NotFound(new { message = "User board not found." });
            }

            _logger.LogDebug("User {UserId} to board {BoardId} retrieved", UserId, boardId);
            return Ok(userBoard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId} to board {BoardId}", UserId, boardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
    
    [HttpPost("user/{userToAddId:int}/board/{boardId:int}")]
    public async Task<IActionResult> AddUserBoard(int userToAddId, int boardId)
    {
        try
        {
            OutputUserBoardDto? userBoard = await _userBoardService.AddUserBoard(userToAddId, boardId, UserId);
            if (userBoard == null)
            {
                _logger.LogError("Failed to add user {UserBoard} to board {BoardId}", userToAddId, boardId);
                return BadRequest(new { message = "Failed to add user." });
            }
            
            _logger.LogInformation("User {UserId} added to board {BoardId}", userToAddId, boardId);
            return CreatedAtAction(nameof(GetUserBoardById), new { boardId = userBoard.BoardId }, userBoard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user {UserId} for board {BoardId}", userToAddId, boardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpDelete("user/{userToDeleteId:int}/board/{boardId:int}")]
    public async Task<IActionResult> DeleteUserBoard(int userToDeleteId, int boardId)
    {
        try
        {
            OutputUserBoardDto? userBoard = await _userBoardService.DeleteUserBoard(userToDeleteId, boardId, UserId);
            if (userBoard == null)
            {
                _logger.LogDebug("User {UserId} for board {BoardId} not found for deletion.", userToDeleteId, boardId);
                return NotFound(new { message = "UserBoard not found." });
            }

            _logger.LogInformation("User {UserId} deleted for board {BoardId}.", userToDeleteId, boardId);
            return Ok(userBoard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId} for board {BoardId}.", userToDeleteId, boardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}