using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using TrelloApi.Application.Hub;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.Constants;
using TrelloApi.Domain.DTOs.Board;
using TrelloApi.Domain.DTOs.Label;

namespace TrelloApi.Application.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
[EnableRateLimiting("fixed")]
public class LabelController : BaseController
{
    private readonly ILogger<LabelController> _logger;
    private readonly ILabelService _labelService;
    private readonly IHubContext<BoardHub> _hubContext;
    private readonly IAuthorizationService _authorizationService;
    private readonly IBoardService _boardService;

    public LabelController(ILogger<LabelController> logger, ILabelService labelService, IBoardService boardService, IHubContext<BoardHub> hubContext, IAuthorizationService authorizationService)
    {
        _logger = logger;
        _labelService = labelService;
        _hubContext = hubContext;
        _authorizationService = authorizationService;
        _boardService = boardService;
    }

    [HttpGet("{labelId:int}")]
    public async Task<IActionResult> GetLabelById(int labelId)
    {
        var authorize = await TryAuthorizeLabelAsync(labelId);
        if (authorize != null)
        {
            return authorize;
        }
            
        LabelResponse? label = await _labelService.GetLabelById(labelId);
        if (label == null)
        {
            _logger.LogDebug("Label {LabelId} not found", labelId);
            return NotFound(new { message = "Label not found." });
        }

        _logger.LogDebug("Label {LabelId} retrieved", labelId);
        return Ok(label);
    }
    
    [HttpGet("board/{boardId:int}")]
    public async Task<IActionResult> GetLabelsByBoardId(int boardId)
    {
        var authorize = await TryAuthorizeBoardAsync(boardId);
        if (authorize != null)
        {
            return authorize;
        }
            
        List<LabelResponse> labels = await _labelService.GetLabelsByBoardId(boardId);
        _logger.LogDebug("Retrieved {Count} labels for board {BoardId}", labels.Count, boardId);
        return Ok(labels);
    }
    
    [HttpGet("colors")]
    public Task<IActionResult> GetLabelColors()
    {
        List<string> labelColorsAllowed = LabelColorValues.LabelColorsAllowed;
        _logger.LogDebug("Retrieved {Count} colors for label", labelColorsAllowed.Count);
        return Task.FromResult<IActionResult>(Ok(labelColorsAllowed));
    }

    [HttpPost("board/{boardId:int}")]
    public async Task<IActionResult> AddLabel(int boardId, [FromBody] AddLabelDto dto)
    {
        var authorize = await TryAuthorizeBoardAsync(boardId);
        if (authorize != null)
        {
            return authorize;
        }
            
        LabelResponse label = await _labelService.AddLabel(boardId, dto);
            
        await _hubContext.Clients.Group(BoardId.ToString())
            .SendAsync("LabelCreated", label);
            
        _logger.LogInformation("Label added to board {BoardId}", boardId);
        return CreatedAtAction(nameof(GetLabelById), new { labelId = label.Id }, label);
    }

    [HttpPut("{labelId:int}")]
    public async Task<IActionResult> UpdateLabel(int labelId, [FromBody] UpdateLabelDto dto)
    {
        var authorize = await TryAuthorizeLabelAsync(labelId);
        if (authorize != null)
        {
            return authorize;
        }
            
        LabelResponse? label = await _labelService.UpdateLabel(labelId, dto);
        if (label == null)
        {
            _logger.LogDebug("Label {LabelId} not found for update", labelId);
            return NotFound(new { message = "Label not found." });
        }
            
        await _hubContext.Clients.Group(BoardId.ToString())
            .SendAsync("LabelUpdated", label);
            
        _logger.LogInformation("Label {LabelId} updated", labelId);
        return Ok(label);
    }

    [HttpDelete("{labelId:int}")]
    public async Task<IActionResult> DeleteLabel(int labelId)
    {
        var authorize = await TryAuthorizeLabelAsync(labelId);
        if (authorize != null)
        {
            return authorize;
        }
            
        Boolean isDeleted = await _labelService.DeleteLabel(labelId);
        if (!isDeleted)
        {
            _logger.LogDebug("Label {LabelId} not found for deletion", labelId);
            return NotFound(new { message = "Label not found." });
        }

        await _hubContext.Clients.Group(BoardId.ToString())
            .SendAsync("LabelDeleted", labelId);
            
        _logger.LogInformation("Label {LabelId} deleted", labelId);
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
    
    private async Task<IActionResult?> TryAuthorizeLabelAsync (int labelId)
    {
        LabelResponse? label = await _labelService.GetLabelByIdToAccess(labelId);
        if (label == null)
        {
            _logger.LogDebug("Label {LabelId} not found for access.", labelId);
            return NotFound(new { message = "Label not found." });
        }
        
        var authResult = await _authorizationService.AuthorizeAsync(User, label.Board.Id, "BoardAccessPolicy");
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        return null;
    }
}