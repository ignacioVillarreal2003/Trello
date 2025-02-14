using Microsoft.AspNetCore.Mvc;
using TrelloApi.Application.Services;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Application.Controllers;

[ApiController]
[Route("[controller]")]
public class UserTaskController: BaseController
{
    private readonly ILogger<UserTaskController> _logger;
    private readonly IUserCardService _userCardService;

    public UserTaskController(ILogger<UserTaskController> logger, IUserCardService userCardService)
    {
        _logger = logger;
        _userCardService = userCardService;
    }

    [HttpGet("{taskId:int}")]
    public async Task<IActionResult> GetUserCardById(int taskId)
    {
        try
        {
            OutputUserCardDto? userTask = await _userCardService.GetUserCardById(UserId, taskId);
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
    public async Task<IActionResult> AddUserTask([FromBody] AddUserCardDto addUserTaskDto)
    {
        try
        {
            OutputUserCardDto? userCard = await _userCardService.AddUserCard(addUserTaskDto, UserId);
            if (userCard == null)
            {
                _logger.LogError("Failed to add user {UserBoard} to card {CardId}",  addUserTaskDto.UserId, addUserTaskDto.CardId);
                return BadRequest(new { message = "Failed to add user." });
            }
            
            _logger.LogInformation("User {UserBoard} added to card {CardId}", addUserTaskDto.UserId, addUserTaskDto.CardId);
            return CreatedAtAction(nameof(GetUserCardById), new { userId = userCard.UserId, taskId = userCard.CardId }, userCard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding card {CardId} to user {UserId}", addUserTaskDto.CardId, addUserTaskDto.UserId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpDelete("user/{userToDeleteId:int}/card/{cardId:int}")]
    public async Task<IActionResult> DeleteUserTask(int userToDeleteId, int cardId)
    {
        try
        {
            OutputUserCardDto? userCard = await _userCardService.DeleteUserCard(userToDeleteId, cardId, UserId);
            if (userCard == null)
            {
                _logger.LogDebug("User {UserId} for card {CardId} not found for deletion.", userToDeleteId, cardId);
                return NotFound(new { message = "UserTask not found." });
            }

            _logger.LogInformation("User {UserId} deleted for card {CardId}.", userToDeleteId, cardId);
            return Ok(userCard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId} for card {CardId}.", userToDeleteId, cardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}