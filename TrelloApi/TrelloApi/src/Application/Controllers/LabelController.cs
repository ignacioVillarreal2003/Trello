using Microsoft.AspNetCore.Mvc;
using TrelloApi.Application.Filters;
using TrelloApi.Domain.Constants;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Application.Controllers;

[ApiController]
[Route("[controller]")]
[RequireAuthentication]
public class LabelController : BaseController
{
    private readonly ILogger<LabelController> _logger;
    private readonly ILabelService _labelService;

    public LabelController(ILogger<LabelController> logger, ILabelService labelService)
    {
        _logger = logger;
        _labelService = labelService;
    }

    [HttpGet("{labelId:int}")]
    public async Task<IActionResult> GetLabelById(int labelId)
    {
        try
        {
            OutputLabelDetailsDto? label = await _labelService.GetLabelById(labelId, UserId);
            if (label == null)
            {
                _logger.LogDebug("Label {LabelId} not found", labelId);
                return NotFound(new { message = "Label not found." });
            }

            _logger.LogDebug("Label {LabelId} retrieved", labelId);
            return Ok(label);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving label {LabelId}", labelId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
    
    [HttpGet("board/{boardId:int}")]
    public async Task<IActionResult> GetLabelsByBoardId(int boardId)
    {
        try
        {
            List<OutputLabelDetailsDto> labels = await _labelService.GetLabelsByBoardId(boardId, UserId);
            _logger.LogDebug("Retrieved {Count} labels for board {BoardId}", labels.Count, boardId);
            return Ok(labels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving labels for board {BoardId}", boardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
    
    [HttpGet("colors")]
    public Task<IActionResult> GetLabelColors()
    {
        try
        {
            List<string> labelColorsAllowed = LabelColorValues.LabelColorsAllowed;
            _logger.LogDebug("Retrieved {Count} colors for label", labelColorsAllowed.Count);
            return Task.FromResult<IActionResult>(Ok(labelColorsAllowed));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving colors for label");
            return Task.FromResult<IActionResult>(StatusCode(500, new { message = "An unexpected error occurred." }));
        }
    }

    [HttpPost("board/{boardId:int}")]
    public async Task<IActionResult> AddLabel(int boardId, [FromBody] AddLabelDto dto)
    {
        try
        {
            OutputLabelDetailsDto? label = await _labelService.AddLabel(boardId, dto, UserId);
            if (label == null)
            {
                _logger.LogError("Failed to add label to board {BoardId}", boardId);
                return BadRequest(new { message = "Failed to add label." });
            }

            _logger.LogInformation("Label added to board {BoardId}", boardId);
            return CreatedAtAction(nameof(GetLabelById), new { labelId = label.Id }, label);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding label to board {BoardId}", boardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpPut("{labelId:int}")]
    public async Task<IActionResult> UpdateLabel(int labelId, [FromBody] UpdateLabelDto dto)
    {
        try
        {
            OutputLabelDetailsDto? label = await _labelService.UpdateLabel(labelId, dto, UserId);
            if (label == null)
            {
                _logger.LogDebug("Label {LabelId} not found for update", labelId);
                return NotFound(new { message = "Label not found." });
            }
            
            _logger.LogInformation("Label {LabelId} updated", labelId);
            return Ok(label);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating label {LabelId}", labelId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpDelete("{labelId:int}")]
    public async Task<IActionResult> DeleteLabel(int labelId)
    {
        try
        {
            Boolean isDeleted = await _labelService.DeleteLabel(labelId, UserId);
            if (!isDeleted)
            {
                _logger.LogDebug("Label {LabelId} not found for deletion", labelId);
                return NotFound(new { message = "Label not found." });
            }

            _logger.LogInformation("Label {LabelId} deleted", labelId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting label {LabelId}", labelId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}