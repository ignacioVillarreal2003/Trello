using Microsoft.AspNetCore.Mvc;
using TrelloApi.Application.Filters;
using TrelloApi.Application.Services;
using TrelloApi.Domain.Board;
using TrelloApi.Domain.Entities.Board;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Application.Controllers;

[ApiController]
[Route("[controller]")]
[RequireAuthentication]
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
            OutputBoardDto? board = await _boardService.GetBoardById(boardId, UserId);
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
    public async Task<IActionResult> GetBoards()
    {
        try
        {
            List<OutputBoardDto> boards = await _boardService.GetBoards(UserId);
            _logger.LogDebug("Retrieved {Count} boards for user {UserId}", boards.Count, UserId);
            return Ok(boards);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving boards for user {UserId}", UserId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddBoard([FromBody] AddBoardDto addBoardDto)
    {
        try
        {
            OutputBoardDto? board = await _boardService.AddBoard(addBoardDto, UserId);
            if (board == null)
            {
                _logger.LogError("Failed to add board to user {UserId}", UserId);
                return BadRequest(new { message = "Failed to add board." });
            }

            _logger.LogInformation("Board added to user {UserId}", UserId);
            return CreatedAtAction(nameof(GetBoardById), new { boardId = board.Id }, board);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding board to user {UserId}", UserId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpPut("{boardId:int}")]
    public async Task<IActionResult> UpdateBoard(int boardId, [FromBody] UpdateBoardDto updateBoardDto)
    {
        try
        {
            OutputBoardDto? board = await _boardService.UpdateBoard(boardId, updateBoardDto, UserId);
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
            OutputBoardDto? board = await _boardService.DeleteBoard(boardId, UserId);
            if (board == null)
            {
                _logger.LogDebug("Board {BoardId} not found for deletion", boardId);
                return NotFound(new { message = "Board not found." });
            }

            _logger.LogInformation("Board {BoardId} deleted", boardId);
            return Ok(board);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting board {BoardId}", boardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}
