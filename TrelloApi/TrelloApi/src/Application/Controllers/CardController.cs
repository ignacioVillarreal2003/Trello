using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using TrelloApi.Application.Hub;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.Constants;
using TrelloApi.Domain.DTOs.Card;
using TrelloApi.Domain.DTOs.List;

namespace TrelloApi.Application.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
[EnableRateLimiting("fixed")]
public class CardController: BaseController
{
    private readonly ILogger<CardController> _logger;
    private readonly ICardService _cardService;
    private readonly IListService _listService;
    private readonly IHubContext<BoardHub> _hubContext;
    private readonly IAuthorizationService _authorizationService;

    public CardController(ILogger<CardController> logger, ICardService cardService, IListService listService, IHubContext<BoardHub> hubContext, IAuthorizationService authorizationService)
    {
        _logger = logger;
        _cardService = cardService;
        _hubContext = hubContext;
        _authorizationService = authorizationService;
        _listService = listService;
    }

    [HttpGet("{cardId:int}")]
    public async Task<IActionResult> GetCardById(int cardId)
    {
        try
        {
            CardResponse? cardToAccess = await _cardService.GetCardByIdToAccess(cardId);
            if (cardToAccess == null)
            {
                _logger.LogDebug("Card {CardId} not found for got.", cardId);
                return NotFound(new { message = "Card not found." });
            }
        
            var authResult = await _authorizationService.AuthorizeAsync(User, cardToAccess.List.BoardId, "BoardAccessPolicy");
            if (!authResult.Succeeded)
            {
                return Forbid();
            }
            
            CardResponse? card = await _cardService.GetCardById(cardId);
            if (card == null)
            {
                _logger.LogDebug("Card {CardId} not found", cardId);
                return NotFound(new { message = "Card not found." });
            }
            
            _logger.LogDebug("Card {CardId} retrieved", cardId);
            return Ok(card);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving card {cardId}", cardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpGet("list/{listId:int}")]
    public async Task<IActionResult> GetCardsByListId(int listId)
    {
        try
        {
            ListResponse? listToAccess = await _listService.GetListByIdToAccess(listId);
            if (listToAccess == null)
            {
                _logger.LogDebug("List {ListId} not found for got.", listId);
                return NotFound(new { message = "List not found." });
            }
        
            var authResult = await _authorizationService.AuthorizeAsync(User, listToAccess.BoardId, "BoardAccessPolicy");
            if (!authResult.Succeeded)
            {
                return Forbid();
            }
            
            List<CardResponse> cards = await _cardService.GetCardsByListId(listId);
            _logger.LogDebug("Retrieved {Count} cards for list {ListId}", cards.Count, listId);
            return Ok(cards);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cards for list {ListId}", listId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
    
    [HttpPost("list/{listId:int}")]
    public async Task<IActionResult> AddCard(int listId, [FromBody] AddCardDto dto)
    {
        try
        {
            ListResponse? listToAccess = await _listService.GetListByIdToAccess(listId);
            if (listToAccess == null)
            {
                _logger.LogDebug("List {ListId} not found for added.", listId);
                return NotFound(new { message = "List not found." });
            }
        
            var authResult = await _authorizationService.AuthorizeAsync(User, listToAccess.BoardId, "BoardAccessPolicy");
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            CardResponse card = await _cardService.AddCard(listId, dto);
            
            await _hubContext.Clients.Group(BoardId.ToString())
                .SendAsync("CardCreated", card);
            
            _logger.LogInformation("Card added to list {ListId}", listId);
            return CreatedAtAction(nameof(GetCardById), new { cardId = card.Id }, card);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding card to list {ListId}", listId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpPut("{cardId:int}")]
    public async Task<IActionResult> UpdateCard(int cardId, [FromBody] UpdateCardDto dto)
    {
        try
        {
            CardResponse? cardToAccess = await _cardService.GetCardByIdToAccess(cardId);
            if (cardToAccess == null)
            {
                _logger.LogDebug("Card {CardId} not found for updated.", cardId);
                return NotFound(new { message = "Card not found." });
            }
        
            var authResult = await _authorizationService.AuthorizeAsync(User, cardToAccess.List.BoardId, "BoardAccessPolicy");
            if (!authResult.Succeeded)
            {
                return Forbid();
            }
            
            CardResponse? card = await _cardService.UpdateCard(cardId, dto);
            if (card == null)
            {
                _logger.LogDebug("Card {CardId} not found for update", cardId);
                return NotFound(new { message = "Card not found." });
            }
            
            await _hubContext.Clients.Group(BoardId.ToString())
                .SendAsync("CardUpdated", card);
            
            _logger.LogInformation("Card {CardId} updated", cardId);
            return Ok(card);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating card {CardId}.", cardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpDelete("{cardId:int}")]
    public async Task<IActionResult> DeleteCard(int cardId)
    {
        try
        {
            CardResponse? cardToAccess = await _cardService.GetCardByIdToAccess(cardId);
            if (cardToAccess == null)
            {
                _logger.LogDebug("Card {CardId} not found for deletion.", cardId);
                return NotFound(new { message = "Card not found." });
            }
        
            var authResult = await _authorizationService.AuthorizeAsync(User, cardToAccess.List.BoardId, "BoardAccessPolicy");
            if (!authResult.Succeeded)
            {
                return Forbid();
            }
            
            Boolean isDeleted = await _cardService.DeleteCard(cardId);
            if (!isDeleted)
            {
                _logger.LogDebug("Card {CardId} not found for deletion.", cardId);
                return NotFound(new { message = "Card not found." });
            }
            
            await _hubContext.Clients.Group(BoardId.ToString())
                .SendAsync("CardDeleted", cardId);

            _logger.LogInformation("Card {CardId} deleted", cardId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting card {CardId}.", cardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}