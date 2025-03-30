using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using TrelloApi.Application.Hub;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.Card;
using TrelloApi.Domain.DTOs.CardLabel;
using TrelloApi.Domain.DTOs.Label;

namespace TrelloApi.Application.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
[EnableRateLimiting("fixed")]
public class CardLabelController: BaseController
{
    private readonly ILogger<CardLabelController> _logger;
    private readonly ICardLabelService _cardLabelService;
    private readonly ICardService _cardService;
    private readonly IHubContext<BoardHub> _hubContext;
    private readonly IAuthorizationService _authorizationService;

    public CardLabelController(ILogger<CardLabelController> logger, ICardLabelService cardLabelService, ICardService cardService, IHubContext<BoardHub> hubContext, IAuthorizationService authorizationService)
    {
        _logger = logger;
        _cardLabelService = cardLabelService;
        _hubContext = hubContext;
        _cardService = cardService;
        _authorizationService = authorizationService;
    }

    [HttpGet("card/{cardId:int}")]
    public async Task<IActionResult> GetLabelsByCardId(int cardId)
    {
        var authorize = await TryAuthorizeCardAsync(cardId);
        if (authorize != null)
        {
            return authorize;
        }
            
        List<LabelResponse> labels = await _cardLabelService.GetLabelsByCardId(cardId);
        _logger.LogDebug("Retrieved {Count} labels for card {CardId}", labels.Count, cardId);
        return Ok(labels);
    }
    
    [HttpPost("card/{cardId:int}")]
    public async Task<IActionResult> AddLabelToCard(int cardId, AddCardLabelDto dto)
    {
        var authorize = await TryAuthorizeCardAsync(cardId);
        if (authorize != null)
        {
            return authorize;
        }
            
        CardLabelResponse cardLabel = await _cardLabelService.AddLabelToCard(cardId, dto);
            
        await _hubContext.Clients.Group(BoardId.ToString())
            .SendAsync("CardLabelCreated", cardLabel);
            
        _logger.LogInformation("Label {LabelId} added to card {CardId}", dto, cardId);
        return CreatedAtAction(nameof(GetLabelsByCardId), new { cardId = cardLabel.CardId, labelId = cardLabel.LabelId }, cardLabel);
    }

    [HttpDelete("card/{cardId:int}/label/{labelId:int}")]
    public async Task<IActionResult> RemoveLabelFromCard(int cardId, int labelId)
    {
        var authorize = await TryAuthorizeCardAsync(cardId);
        if (authorize != null)
        {
            return authorize;
        }
            
        bool isDeleted = await _cardLabelService.RemoveLabelFromCard(cardId, labelId);
        if (!isDeleted)
        {
            _logger.LogDebug("Label {LabelId} to card {CardId} not found for deletion.", labelId, cardId);
            return NotFound(new { message = "CardLabel not found." });
        }

        await _hubContext.Clients.Group(BoardId.ToString())
            .SendAsync("CardLabelDeleted", cardId, labelId);
            
        _logger.LogInformation("Label {LabelId} deleted for card {CardId}.", labelId, cardId);
        return NoContent();
    }
    
    private async Task<IActionResult?> TryAuthorizeCardAsync (int cardId)
    {
        CardResponse? card = await _cardService.GetCardByIdToAccess(cardId);
        if (card == null)
        {
            _logger.LogDebug("Card {CardId} not found for access.", cardId);
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