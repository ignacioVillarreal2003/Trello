using Microsoft.AspNetCore.Mvc;
using TrelloApi.Domain.Entities.TaskLabel;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Application.Controllers;

[ApiController]
[Route("[controller]")]
public class TaskLabelController: BaseController
{
    private readonly ILogger<TaskLabelController> _logger;
    private readonly ITaskLabelService _taskLabelService;

    public TaskLabelController(ILogger<TaskLabelController> logger, ITaskLabelService taskLabelService)
    {
        _logger = logger;
        _taskLabelService = taskLabelService;
    }

    [HttpGet("task/{taskId:int}/label/{labelId:int}")]
    public async Task<IActionResult> GetTaskLabelById(int taskId, int labelId)
    {
        try
        {
            OutputTaskLabelDto? taskLabel = await _taskLabelService.GetTaskLabelById(taskId, labelId, UserId);
            if (taskLabel == null)
            {
                _logger.LogDebug("Label {LabelId} to task {TaskId} not found", labelId, taskId);
                return NotFound(new { message = "TaskLabel not found." });
            }

            _logger.LogDebug("Label {LabelId} to task {TaskId} retrieved", labelId, taskId);
            return Ok(taskLabel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving label {LabelId} to task {TaskId}", labelId, taskId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
    
    [HttpPost("task/{taskId:int}/label/{labelId:int}")]
    public async Task<IActionResult> AddTaskLabel(int taskId, int labelId)
    {
        try
        {
            OutputTaskLabelDto? taskLabel = await _taskLabelService.AddTaskLabel(taskId, labelId, UserId);
            if (taskLabel == null)
            {
                _logger.LogError("Failed to add label {LabelId} to task {TaskId}", labelId, taskId);
                return BadRequest(new { message = "Failed to add task label." });
            }
            
            _logger.LogInformation("Label {LabelId} added to task {TaskId}", labelId, taskId);
            return CreatedAtAction(nameof(GetTaskLabelById), new { taskId = taskLabel.TaskId, labelId = taskLabel.LabelId }, taskLabel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding label {LabelId} to task {TaskId}", labelId, taskId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpDelete("task/{taskId:int}/label/{labelId:int}")]
    public async Task<IActionResult> DeleteTaskLabel(int taskId, int labelId)
    {
        try
        {
            OutputTaskLabelDto? taskLabel = await _taskLabelService.DeleteTaskLabel(taskId, labelId, UserId);
            if (taskLabel == null)
            {
                _logger.LogDebug("Label {LabelId} to task {TaskId} not found for deletion.", labelId, taskId);
                return NotFound(new { message = "TaskLabel not found." });
            }

            _logger.LogInformation("Label {LabelId} deleted for task {TaskId}.", labelId, taskId);
            return Ok(taskLabel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting label {LabelId} for task {TaskId}.", labelId, taskId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}