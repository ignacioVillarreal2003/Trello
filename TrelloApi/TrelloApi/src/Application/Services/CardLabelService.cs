using AutoMapper;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Application.Services;

public class CardLabelService: BaseService, ICardLabelService
{
    private readonly ILogger<CardLabelService> _logger;
    private readonly ICardLabelRepository _cardLabelRepository;
    
    public CardLabelService(IMapper mapper, IBoardAuthorizationService boardAuthorizationService, ILogger<CardLabelService> logger, ICardLabelRepository cardLabelRepository) : base(mapper, boardAuthorizationService)
    {
        _logger = logger;
        _cardLabelRepository = cardLabelRepository;
    }

    public async Task<List<OutputLabelDetailsDto>> GetLabelsByCardId(int cardId, int uid)
    {
        try
        {
            List<Label> labels = await _cardLabelRepository.GetLabelsByCardId(cardId);
            _logger.LogDebug("Retrieved {Count} labels for card {CardId}", labels.Count, cardId);
            return _mapper.Map<List<OutputLabelDetailsDto>>(labels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving labels for card {CardId}", cardId);
            throw;
        }
    }

    public async Task<OutputCardLabelListDto?> AddLabelToCard(int cardId, int labelId, int uid)
    {
        try
        {
            CardLabel cardLabel = new CardLabel(cardId, labelId);
            CardLabel? newCardLabel = await _cardLabelRepository.AddCardLabel(cardLabel);
            if (newCardLabel == null)
            {
                _logger.LogError("Failed to add label {LabelId} to card {CardId}", labelId, cardId);
                return null;
            }
            
            _logger.LogInformation("Label {LabelId} added to card {CardId}", labelId, cardId);
            return _mapper.Map<OutputCardLabelListDto>(newCardLabel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding label {LabelId} for card {CardId}", labelId, cardId);
            throw;
        }
    }

    public async Task<Boolean> RemoveLabelFromCard(int cardId, int labelId, int uid)
    {
        try
        {
            CardLabel? cardLabel = await _cardLabelRepository.GetCardLabelById(cardId, labelId);
            if (cardLabel == null)
            {
                _logger.LogWarning("Label {LabelId} for card {CardId} not found for deletion", labelId, cardId);
                return false;
            }

            CardLabel? deletedCardLabel = await _cardLabelRepository.DeleteCardLabel(cardLabel);
            if (deletedCardLabel == null)
            {
                _logger.LogError("Failed to delete label {LabelId} for card {CardId}", labelId, cardId);
                return false;
            }

            _logger.LogInformation("Label {LabelId} for card {CardId} deleted", labelId, cardId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting label {LabelId} for card {CardId}", labelId, cardId);
            throw;
        }
    }
}