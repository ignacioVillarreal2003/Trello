using Microsoft.AspNetCore.Mvc;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Application.Controllers;

[ApiController]
[Route("[controller]")]
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
            List<OutputLabelDetailsDto> labels = await _cardLabelService.GetLabelsByCardId(cardId, UserId);
            _logger.LogDebug("Retrieved {Count} labels for card {CardId}", labels.Count, cardId);
            return Ok(labels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving labels to card {CardId}", cardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
    
    [HttpPost("card/{cardId:int}/label/{labelId:int}")]
    public async Task<IActionResult> AddLabelToCard(int cardId, int labelId)
    {
        try
        {
            OutputCardLabelListDto? cardLabel = await _cardLabelService.AddLabelToCard(cardId, labelId, UserId);
            if (cardLabel == null)
            {
                _logger.LogError("Failed to add label {LabelId} to card {CardId}", labelId, cardId);
                return BadRequest(new { message = "Failed to add card label." });
            }
            
            _logger.LogInformation("Label {LabelId} added to card {CardId}", labelId, cardId);
            return CreatedAtAction(nameof(GetLabelsByCardId), new { cardId = cardLabel.CardId, labelId = cardLabel.LabelId }, cardLabel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding label {LabelId} to card {CardId}", labelId, cardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpDelete("card/{cardId:int}/label/{labelId:int}")]
    public async Task<IActionResult> RemoveLabelFromCard(int cardId, int labelId)
    {
        try
        {
            bool isDeleted = await _cardLabelService.RemoveLabelFromCard(cardId, labelId, UserId);
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