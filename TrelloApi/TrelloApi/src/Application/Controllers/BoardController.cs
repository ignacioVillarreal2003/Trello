using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.Constants;
using TrelloApi.Domain.DTOs.Board;

namespace TrelloApi.Application.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
[EnableRateLimiting("fixed")]
public class BoardController : BaseController
{
    private readonly ILogger<BoardController> _logger;
    private readonly IBoardService _boardService;

    public BoardController(ILogger<BoardController> logger, IBoardService boardService)
    {
        _logger = logger;
        _boardService = boardService;
    }

    [HttpGet("{boardId:int}")]
    public async Task<IActionResult> GetBoardById(int boardId)
    {
        try
        {
            BoardResponse? board = await _boardService.GetBoardById(boardId);
            if (board == null)
            {
                _logger.LogDebug("Board {BoardId} not found", boardId);
                return NotFound(new { message = "Board not found." });
            }

            _logger.LogDebug("Board {BoardId} retrieved", boardId);
            return Ok(board);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving board {BoardId}", boardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetBoardsByUserId()
    {
        try
        {
            List<BoardResponse> boards = await _boardService.GetBoardsByUserId(UserId);
            _logger.LogDebug("Retrieved {Count} boards for user {UserId}", boards.Count, UserId);
            return Ok(boards);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving boards for user {UserId}", UserId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpGet("colors")]
    public Task<IActionResult> GetBoardColors()
    {
        try
        {
            List<string> boardBackgroundsAllowed = BoardBackgroundValues.BoardBackgroundsAllowed;
            _logger.LogDebug("Retrieved {Count} backgrounds for board", boardBackgroundsAllowed.Count);
            return Task.FromResult<IActionResult>(Ok(boardBackgroundsAllowed));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving backgrounds for board");
            return Task.FromResult<IActionResult>(StatusCode(500, new { message = "An unexpected error occurred." }));
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> AddBoard([FromBody] AddBoardDto dto)
    {
        try
        {
            BoardResponse board = await _boardService.AddBoard(dto, UserId);
            _logger.LogInformation("Board added for user {UserId}", UserId);
            return CreatedAtAction(nameof(GetBoardById), new { boardId = board.Id }, board);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding board for user {UserId}", UserId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpPut("{boardId:int}")]
    public async Task<IActionResult> UpdateBoard(int boardId, [FromBody] UpdateBoardDto dto)
    {
        try
        {
            BoardResponse? board = await _boardService.UpdateBoard(boardId, dto);
            if (board == null)
            {
                _logger.LogDebug("Board {BoardId} not found for update", boardId);
                return NotFound(new { message = "Board not found." });
            }
        
            _logger.LogInformation("Board {BoardId} updated", boardId);
            return Ok(board);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating board {BoardId}", boardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
    
    [HttpDelete("{boardId:int}")]
    public async Task<IActionResult> DeleteBoard(int boardId)
    {
        try
        {
            bool isDeleted = await _boardService.DeleteBoard(boardId);
            if (!isDeleted)
            {
                _logger.LogDebug("Board {BoardId} not found for deletion", boardId);
                return NotFound(new { message = "Board not found." });
            }

            _logger.LogInformation("Board {BoardId} deleted", boardId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting board {BoardId}", boardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}
