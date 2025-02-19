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

    public async Task<OutputCardLabelDetailsDto?> AddLabelToCard(int cardId, AddCardLabelDto dto, int uid)
    {
        try
        {
            CardLabel cardLabel = new CardLabel(cardId, dto.LabelId);
            await _cardLabelRepository.AddCardLabel(cardLabel);
            
            _logger.LogInformation("Label {LabelId} added to card {CardId}", dto.LabelId, cardId);
            return _mapper.Map<OutputCardLabelDetailsDto>(cardLabel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding label {LabelId} for card {CardId}", dto.LabelId, cardId);
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

            await _cardLabelRepository.DeleteCardLabel(cardLabel);

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