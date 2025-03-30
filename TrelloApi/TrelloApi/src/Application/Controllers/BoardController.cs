using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using TrelloApi.Application.Hub;
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
    private readonly IHubContext<BoardHub> _hubContext;
    private readonly IAuthorizationService _authorizationService;
    
    public BoardController(ILogger<BoardController> logger, IBoardService boardService, IHubContext<BoardHub> hubContext, IAuthorizationService authorizationService)
    {
        _logger = logger;
        _boardService = boardService;
        _hubContext = hubContext;
        _authorizationService = authorizationService;
    }

    [HttpGet("{boardId:int}")]
    public async Task<IActionResult> GetBoardById(int boardId)
    {
        var authorize = await TryAuthorizeBoardAsync(boardId);
        if (authorize != null)
        {
            return authorize;
        }
            
        BoardResponse? board = await _boardService.GetBoardById(boardId);
        if (board == null)
        {
            _logger.LogDebug("Board {BoardId} not found", boardId);
            return NotFound(new { message = "Board not found." });
        }

        _logger.LogDebug("Board {BoardId} retrieved", boardId);
        return Ok(board);
    }

    [HttpGet("{boardId:int}/complete")]
    public async Task<IActionResult> GetBoardByIdComplete(int boardId)
    {
        var authorize = await TryAuthorizeBoardAsync(boardId);
        if (authorize != null)
        {
            return authorize;
        }
            
        BoardResponse? board = await _boardService.GetBoardByIdComplete(boardId);
        if (board == null)
        {
            _logger.LogDebug("Board {BoardId} not found", boardId);
            return NotFound(new { message = "Board not found." });
        }

        _logger.LogDebug("Board {BoardId} retrieved", boardId);
        return Ok(board);
    }

    [HttpGet]
    public async Task<IActionResult> GetBoardsByUserId()
    {
        List<BoardResponse> boards = await _boardService.GetBoardsByUserId(UserId);
        _logger.LogDebug("Retrieved {Count} boards for user {UserId}", boards.Count, UserId);
        return Ok(boards);
    }

    [HttpGet("background")]
    public Task<IActionResult> GetBoardBackgrounds()
    {
        List<string> boardBackgroundsAllowed = BoardBackgroundValues.BoardBackgroundsAllowed;
        _logger.LogDebug("Retrieved {Count} backgrounds for board", boardBackgroundsAllowed.Count);
        return Task.FromResult<IActionResult>(Ok(boardBackgroundsAllowed));
    }
    
    [HttpPost]
    public async Task<IActionResult> AddBoard([FromBody] AddBoardDto dto)
    {
        BoardResponse board = await _boardService.AddBoard(dto, UserId);
        _logger.LogInformation("Board added for user {UserId}", UserId);
        return CreatedAtAction(nameof(GetBoardById), new { boardId = board.Id }, board);
    }

    [HttpPut("{boardId:int}")]
    public async Task<IActionResult> UpdateBoard(int boardId, [FromBody] UpdateBoardDto dto)
    {
        var authorize = await TryAuthorizeBoardAsync(boardId);
        if (authorize != null)
        {
            return authorize;
        }
            
        BoardResponse? board = await _boardService.UpdateBoard(boardId, dto);
        if (board == null)
        {
            _logger.LogDebug("Board {BoardId} not found for update", boardId);
            return NotFound(new { message = "Board not found." });
        }
            
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync("BoardUpdated", board);
            
        _logger.LogInformation("Board {BoardId} updated", boardId);
        return Ok(board);
    }
    
    [HttpDelete("{boardId:int}")]
    public async Task<IActionResult> DeleteBoard(int boardId)
    {
        var authorize = await TryAuthorizeBoardAsync(boardId);
        if (authorize != null)
        {
            return authorize;
        }
            
        bool isDeleted = await _boardService.DeleteBoard(boardId);
        if (!isDeleted)
        {
            _logger.LogDebug("Board {BoardId} not found for deletion", boardId);
            return NotFound(new { message = "Board not found." });
        }
            
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync("BoardDeleted", boardId);

        _logger.LogInformation("Board {BoardId} deleted", boardId);
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
