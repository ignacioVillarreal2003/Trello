using Microsoft.AspNetCore.Mvc;
using TrelloApi.Application.Filters;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Interfaces.Services;

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
    public async Task<IActionResult> GetBoardMembers(int boardId)
    {
        try
        {
            List<OutputUserBoardListDto> users = await _userBoardService.GetBoardMembers(boardId, UserId);
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
    public async Task<IActionResult> AddMemberToBoard(int boardId, [FromBody] AddUserBoardDto dto)
    {
        try
        {
            OutputUserBoardDto? userBoard = await _userBoardService.AddMemberToBoard(boardId, dto, UserId);
            if (userBoard == null)
            {
                _logger.LogError("Failed to add user {UserId} to board {BoardId}", dto.UserId, boardId);
                return BadRequest(new { message = "Failed to add user to board." });
            }
        
            _logger.LogInformation("User {UserId} added to board {BoardId}", dto.UserId, boardId);
            return CreatedAtAction(nameof(GetBoardMembers), new { boardId = userBoard.Id }, userBoard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user {UserId} to board {BoardId}", dto.UserId, boardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }


    [HttpDelete("board/{boardId:int}/user/{userId:int}")]
    public async Task<IActionResult> RemoveMemberFromBoard(int boardId, int userId)
    {
        try
        {
            bool isDeleted = await _userBoardService.RemoveMemberFromBoard(boardId, userId, UserId);
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