using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.User;
using TrelloApi.Domain.DTOs.UserCard;

namespace TrelloApi.Application.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
[EnableRateLimiting("fixed")]
public class UserCardController: BaseController
{
    private readonly ILogger<UserCardController> _logger;
    private readonly IUserCardService _userCardService;

    public UserCardController(ILogger<UserCardController> logger, IUserCardService userCardService)
    {
        _logger = logger;
        _userCardService = userCardService;
    }

    [HttpGet("{cardId:int}")]
    public async Task<IActionResult> GetUsersByCardId(int cardId)
    {
        try
        {
            List<UserResponse> users = await _userCardService.GetUsersByCardId(cardId);
            _logger.LogDebug("Retrieved {Count} users for card {CardId}", users.Count, cardId);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users members for card {CardId}", cardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
    
    [HttpPost("card/{cardId:int}")]
    public async Task<IActionResult> AddUserToCard(int cardId, [FromBody] AddUserCardDto dto)
    {
        try
        {
            UserCardResponse? userCard = await _userCardService.AddUserToCard(cardId, dto);
            if (userCard == null)
            {
                _logger.LogError("Failed to add user {UserBoard} to card {CardId}",  dto.UserId, cardId);
                return BadRequest(new { message = "Failed to add user." });
            }
            
            _logger.LogInformation("User {UserBoard} added to card {CardId}", dto.UserId, cardId);
            return CreatedAtAction(nameof(GetUsersByCardId), new { userId = userCard.UserId, taskId = userCard.CardId }, userCard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding card {CardId} to user {UserId}", cardId, dto.UserId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpDelete("user/{userId:int}/card/{cardId:int}")]
    public async Task<IActionResult> RemoveUserFromCard(int userId, int cardId)
    {
        try
        {
            Boolean isDeleted = await _userCardService.RemoveUserFromCard(userId, cardId);
            if (!isDeleted)
            {
                _logger.LogDebug("User {UserId} for card {CardId} not found for deletion.", userId, cardId);
                return NotFound(new { message = "UserTask not found." });
            }

            _logger.LogInformation("User {UserId} deleted for card {CardId}.", userId, cardId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId} for card {CardId}.", userId, cardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}