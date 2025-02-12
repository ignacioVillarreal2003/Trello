using Microsoft.AspNetCore.Mvc;
using TrelloApi.Application.Services;
using TrelloApi.Domain.Entities.UserTask;
using TrelloApi.Domain.Interfaces.Services;
using TrelloApi.Domain.UserTask.Dto;

namespace TrelloApi.Application.Controllers;

[ApiController]
[Route("[controller]")]
public class UserTaskController: BaseController
{
    private readonly ILogger<UserTaskController> _logger;
    private readonly IUserTaskService _userTaskService;

    public UserTaskController(ILogger<UserTaskController> logger, IUserTaskService userTaskService)
    {
        _logger = logger;
        _userTaskService = userTaskService;
    }

    [HttpGet("{taskId:int}")]
    public async Task<IActionResult> GetUserTaskById(int taskId)
    {
        try
        {
            OutputUserTaskDto? userTask = await _userTaskService.GetUserTaskById(UserId, taskId);
            if (userTask == null)
            {
                _logger.LogDebug("Task {TaskId} to user {UserId} not found", taskId, UserId);
                return NotFound(new { message = "User task not found." });
            }

            _logger.LogDebug("Task {TaskId} to user {UserId} retrieved", taskId, UserId);
            return Ok(userTask);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving task {TaskId} to user {UserId}", taskId, UserId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> AddUserTask([FromBody] AddUserTaskDto addUserTaskDto)
    {
        try
        {
            OutputUserTaskDto? userTask = await _userTaskService.AddUserTask(addUserTaskDto, UserId);
            if (userTask == null)
            {
                _logger.LogError("Failed to add user {UserBoard} to task {TaskId}",  addUserTaskDto.UserId, addUserTaskDto.TaskId);
                return BadRequest(new { message = "Failed to add user." });
            }
            
            _logger.LogInformation("User {UserBoard} added to task {TaskId}", addUserTaskDto.UserId, addUserTaskDto.TaskId);
            return CreatedAtAction(nameof(GetUserTaskById), new { userId = userTask.UserId, taskId = userTask.TaskId }, userTask);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding task {TaskId} to user {UserId}", addUserTaskDto.TaskId, addUserTaskDto.UserId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpDelete("user/{userToDeleteId:int}/task/{taskId:int}")]
    public async Task<IActionResult> DeleteUserTask(int userToDeleteId, int taskId)
    {
        try
        {
            OutputUserTaskDto? userTask = await _userTaskService.DeleteUserTask(userToDeleteId, taskId, UserId);
            if (userTask == null)
            {
                _logger.LogDebug("User {UserId} for task {TaskId} not found for deletion.", userToDeleteId, taskId);
                return NotFound(new { message = "UserTask not found." });
            }

            _logger.LogInformation("User {UserId} deleted for task {TaskId}.", userToDeleteId, taskId);
            return Ok(userTask);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId} for task {TaskId}.", userToDeleteId, taskId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}