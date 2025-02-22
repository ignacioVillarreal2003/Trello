using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.Constants;
using TrelloApi.Domain.DTOs.Card;

namespace TrelloApi.Application.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
[EnableRateLimiting("fixed")]
public class CardController: BaseController
{
    private readonly ILogger<CardController> _logger;
    private readonly ICardService _cardService;

    public CardController(ILogger<CardController> logger, ICardService cardService)
    {
        _logger = logger;
        _cardService = cardService;
    }

    [HttpGet("{cardId:int}")]
    public async Task<IActionResult> GetCardById(int cardId)
    {
        try
        {
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
    
    [HttpGet("priorities")]
    public Task<IActionResult> GetBoardColors()
    {
        try
        {
            List<string> prioritiesAllowed = PriorityValues.PrioritiesAllowed;
            _logger.LogDebug("Retrieved {Count} priorities for card", prioritiesAllowed.Count);
            return Task.FromResult<IActionResult>(Ok(prioritiesAllowed));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving colors for board");
            return Task.FromResult<IActionResult>(StatusCode(500, new { message = "An unexpected error occurred." }));
        }
    }
    
    [HttpPost("list/{listId:int}")]
    public async Task<IActionResult> AddCard(int listId, [FromBody] AddCardDto dto)
    {
        try
        {
            CardResponse? card = await _cardService.AddCard(listId, dto);
            if (card == null)
            {
                _logger.LogError("Failed to add card to list {ListId}", listId);
                return BadRequest(new { message = "Failed to add card." });
            }
            
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
            CardResponse? card = await _cardService.UpdateCard(cardId, dto);
            if (card == null)
            {
                _logger.LogDebug("Card {CardId} not found for update", cardId);
                return NotFound(new { message = "Card not found." });
            }
            
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
            Boolean isDeleted = await _cardService.DeleteCard(cardId);
            if (!isDeleted)
            {
                _logger.LogDebug("Card {CardId} not found for deletion.", cardId);
                return NotFound(new { message = "Card not found." });
            }

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