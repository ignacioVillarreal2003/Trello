using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs;
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

    public CardLabelController(ILogger<CardLabelController> logger, ICardLabelService cardLabelService)
    {
        _logger = logger;
        _cardLabelService = cardLabelService;
    }

    [HttpGet("card/{cardId:int}")]
    public async Task<IActionResult> GetLabelsByCardId(int cardId)
    {
        try
        {
            List<LabelResponse> labels = await _cardLabelService.GetLabelsByCardId(cardId);
            _logger.LogDebug("Retrieved {Count} labels for card {CardId}", labels.Count, cardId);
            return Ok(labels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving labels to card {CardId}", cardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
    
    [HttpPost("card/{cardId:int}")]
    public async Task<IActionResult> AddLabelToCard(int cardId, AddCardLabelDto dto)
    {
        try
        {
            CardLabelResponse cardLabel = await _cardLabelService.AddLabelToCard(cardId, dto);
            _logger.LogInformation("Label {LabelId} added to card {CardId}", dto, cardId);
            return CreatedAtAction(nameof(GetLabelsByCardId), new { cardId = cardLabel.CardId, labelId = cardLabel.LabelId }, cardLabel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding label {LabelId} to card {CardId}", dto.LabelId, cardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpDelete("card/{cardId:int}/label/{labelId:int}")]
    public async Task<IActionResult> RemoveLabelFromCard(int cardId, int labelId)
    {
        try
        {
            bool isDeleted = await _cardLabelService.RemoveLabelFromCard(cardId, labelId);
            if (!isDeleted)
            {
                _logger.LogDebug("Label {LabelId} to card {CardId} not found for deletion.", labelId, cardId);
                return NotFound(new { message = "CardLabel not found." });
            }

            _logger.LogInformation("Label {LabelId} deleted for card {CardId}.", labelId, cardId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting label {LabelId} for card {CardId}.", labelId, cardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}