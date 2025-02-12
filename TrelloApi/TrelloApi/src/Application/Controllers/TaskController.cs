using Microsoft.AspNetCore.Mvc;
using TrelloApi.Application.Filters;
using TrelloApi.Application.Services;
using TrelloApi.Domain.Entities.Task;
using TrelloApi.Domain.Interfaces.Services;
using TrelloApi.Domain.Task.DTO;
using Task = TrelloApi.Domain.Task.Task;

namespace TrelloApi.Application.Controllers;

[ApiController]
[Route("[controller]")]
[RequireAuthentication]
public class TaskController: BaseController
{
    private readonly ILogger<TaskController> _logger;
    private readonly ITaskService _taskService;

    public TaskController(ILogger<TaskController> logger, ITaskService taskService)
    {
        _logger = logger;
        _taskService = taskService;
    }

    [HttpGet("{taskId:int}")]
    public async Task<IActionResult> GetTaskById(int taskId)
    {
        try
        {
            OutputTaskDto? task = await _taskService.GetTaskById(taskId, UserId);
            if (task == null)
            {
                _logger.LogDebug("Task {TaskId} not found", taskId);
                return NotFound(new { message = "Task not found." });
            }
            
            _logger.LogDebug("Task {TaskId} retrieved", taskId);
            return Ok(task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving task {TaskId}", taskId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpGet("list/{listId:int}")]
    public async Task<IActionResult> GetTasksByListId(int listId)
    {
        try
        {
            List<OutputTaskDto> tasks = await _taskService.GetTasksByListId(listId, UserId);
            _logger.LogDebug("Retrieved {Count} tasks for list {ListId}", tasks.Count, listId);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks for list {ListId}", listId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpPost("list/{listId:int}")]
    public async Task<IActionResult> AddTask(int listId, [FromBody] AddTaskDto addTaskDto)
    {
        try
        {
            OutputTaskDto? task = await _taskService.AddTask(listId, addTaskDto, UserId);
            if (task == null)
            {
                _logger.LogError("Failed to add task to list {ListId}", listId);
                return BadRequest(new { message = "Failed to add task." });
            }
            
            _logger.LogInformation("Task added to list {ListId}", listId);
            return CreatedAtAction(nameof(GetTaskById), new { taskId = task.Id }, task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding task to list {ListId}", listId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpPut("{taskId:int}")]
    public async Task<IActionResult> UpdateTask(int taskId, [FromBody] UpdateTaskDto updateTaskDto)
    {
        try
        {
            OutputTaskDto? task = await _taskService.UpdateTask(taskId, updateTaskDto, UserId);
            if (task == null)
            {
                _logger.LogDebug("Task {TaskId} not found for update", taskId);
                return NotFound(new { message = "Task not found." });
            }
            
            _logger.LogInformation("Task {TaskId} updated", taskId);
            return Ok(task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task {TaskId}.", taskId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpDelete("{taskId:int}")]
    public async Task<IActionResult> DeleteTask(int taskId)
    {
        try
        {
            OutputTaskDto? task = await _taskService.DeleteTask(taskId, UserId);
            if (task == null)
            {
                _logger.LogDebug("Task {TaskId} not found for deletion.", taskId);
                return NotFound(new { message = "Task not found." });
            }

            _logger.LogInformation("Task {TaskId} deleted", taskId);
            return Ok(task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task {TaskId}.", taskId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}