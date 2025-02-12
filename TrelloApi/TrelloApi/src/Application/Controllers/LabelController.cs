using Microsoft.AspNetCore.Mvc;
using TrelloApi.Application.Filters;
using TrelloApi.Application.Services;
using TrelloApi.Domain.Entities.Label;
using TrelloApi.Domain.Interfaces.Services;
using TrelloApi.Domain.Label;
using TrelloApi.Domain.Label.DTO;

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
            OutputLabelDto? label = await _labelService.GetLabelById(labelId, UserId);
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
    
    [HttpGet("task/{taskId:int}")]
    public async Task<IActionResult> GetLabelsByTaskId(int taskId)
    {
        try
        {
            List<OutputLabelDto> labels = await _labelService.GetLabelsByTaskId(taskId, UserId);
            _logger.LogDebug("Retrieved {Count} labels for task {TaskId}", labels.Count, taskId);
            return Ok(labels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving labels for task {TaskId}", taskId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpPost("board/{boardId:int}")]
    public async Task<IActionResult> AddLabel(int boardId, [FromBody] AddLabelDto addLabelDto)
    {
        try
        {
            OutputLabelDto? label = await _labelService.AddLabel(boardId, addLabelDto, UserId);
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
    public async Task<IActionResult> UpdateLabel(int labelId, [FromBody] UpdateLabelDto updateLabelDto)
    {
        try
        {
            OutputLabelDto? label = await _labelService.UpdateLabel(labelId, updateLabelDto, UserId);
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
            OutputLabelDto? label = await _labelService.DeleteLabel(labelId, UserId);
            if (label == null)
            {
                _logger.LogDebug("Label {LabelId} not found for deletion", labelId);
                return NotFound(new { message = "Label not found." });
            }

            _logger.LogInformation("Label {LabelId} deleted", labelId);
            return Ok(label);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting label {LabelId}", labelId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}