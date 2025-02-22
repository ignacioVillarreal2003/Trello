using AutoMapper;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.DTOs.CardLabel;
using TrelloApi.Domain.DTOs.Label;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Application.Services;

public class CardLabelService: BaseService, ICardLabelService
{
    private readonly ILogger<CardLabelService> _logger;
    private readonly ICardLabelRepository _cardLabelRepository;
    
    public CardLabelService(IMapper mapper, 
        IUnitOfWork unitOfWork, 
        ILogger<CardLabelService> logger, 
        ICardLabelRepository cardLabelRepository) 
        : base(mapper, unitOfWork)
    {
        _logger = logger;
        _cardLabelRepository = cardLabelRepository;
    }

    public async Task<List<LabelResponse>> GetLabelsByCardId(int cardId)
    {
        try
        {
            List<Label> labels = (await _cardLabelRepository.GetLabelsByCardIdAsync(cardId)).ToList();
            _logger.LogDebug("Retrieved {Count} labels for card {CardId}", labels.Count, cardId);
            return _mapper.Map<List<LabelResponse>>(labels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving labels for card {CardId}", cardId);
            throw;
        }
    }

    public async Task<CardLabelResponse?> AddLabelToCard(int cardId, AddCardLabelDto dto)
    {
        try
        {
            CardLabel cardLabel = new CardLabel(cardId, dto.LabelId);
            await _cardLabelRepository.CreateAsync(cardLabel);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Label {LabelId} added to card {CardId}", dto.LabelId, cardId);
            return _mapper.Map<CardLabelResponse>(cardLabel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding label {LabelId} for card {CardId}", dto.LabelId, cardId);
            throw;
        }
    }

    public async Task<Boolean> RemoveLabelFromCard(int cardId, int labelId)
    {
        try
        {
            CardLabel? cardLabel = await _cardLabelRepository.GetAsync(cl => cl.CardId.Equals(cardId) && cl.LabelId.Equals(labelId));
            if (cardLabel == null)
            {
                _logger.LogWarning("Label {LabelId} for card {CardId} not found for deletion", labelId, cardId);
                return false;
            }

            await _cardLabelRepository.DeleteAsync(cardLabel);
            await _unitOfWork.CommitAsync();

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