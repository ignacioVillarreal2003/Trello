using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using TrelloApi.Application.Hub;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.Card;
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
    private readonly IHubContext<BoardHub> _hubContext;
    private readonly ICardService _cardService;
    private readonly IAuthorizationService _authorizationService;
    
    public UserCardController(ILogger<UserCardController> logger, IUserCardService userCardService, ICardService cardService, IHubContext<BoardHub> hubContext, IAuthorizationService authorizationService)
    {
        _logger = logger;
        _userCardService = userCardService;
        _hubContext = hubContext;
        _cardService = cardService;
        _authorizationService = authorizationService;
    }

    [HttpGet("{cardId:int}")]
    public async Task<IActionResult> GetUsersByCardId(int cardId)
    {
        var authorize = await TryAuthorizeCardAsync(cardId);
        if (authorize != null)
        {
            return authorize;
        }
            
        List<UserResponse> users = await _userCardService.GetUsersByCardId(cardId);
        _logger.LogDebug("Retrieved {Count} users for card {CardId}", users.Count, cardId);
        return Ok(users);
    }
    
    [HttpPost("card/{cardId:int}")]
    public async Task<IActionResult> AddUserToCard(int cardId, [FromBody] AddUserCardDto dto)
    {
        var authorize = await TryAuthorizeCardAsync(cardId);
        if (authorize != null)
        {
            return authorize;
        }
            
        UserCardResponse userCard = await _userCardService.AddUserToCard(cardId, dto);
            
        await _hubContext.Clients.Group(BoardId.ToString())
            .SendAsync("UserCardCreated", userCard);
            
        _logger.LogInformation("User {UserBoard} added to card {CardId}", dto.UserId, cardId);
        return CreatedAtAction(nameof(GetUsersByCardId), new { userId = userCard.UserId, cardId = userCard.CardId }, userCard);
    }

    [HttpDelete("user/{userId:int}/card/{cardId:int}")]
    public async Task<IActionResult> RemoveUserFromCard(int userId, int cardId)
    {
        var authorize = await TryAuthorizeCardAsync(cardId);
        if (authorize != null)
        {
            return authorize;
        }
            
        Boolean isDeleted = await _userCardService.RemoveUserFromCard(userId, cardId);
        if (!isDeleted)
        {
            _logger.LogDebug("User {UserId} for card {CardId} not found for deletion.", userId, cardId);
            return NotFound(new { message = "UserCard not found." });
        }

        await _hubContext.Clients.Group(BoardId.ToString())
            .SendAsync("UserCardDeleted", userId, cardId);
            
        _logger.LogInformation("User {UserId} deleted for card {CardId}.", userId, cardId);
        return NoContent();
    }
    
    private async Task<IActionResult?> TryAuthorizeCardAsync (int cardId)
    {
        CardResponse? card = await _cardService.GetCardByIdToAccess(cardId);
        if (card == null)
        {
            _logger.LogDebug("Card {CardId} not found for deletion.", cardId);
            return NotFound(new { message = "Card not found." });
        }
        
        var authResult = await _authorizationService.AuthorizeAsync(User, card.List.BoardId, "BoardAccessPolicy");
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        return null;
    }
}