using AutoMapper;
using TrelloApi.Application.Services.Interfaces;
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
        List<Label> labels = (await _cardLabelRepository.GetLabelsByCardIdAsync(cardId)).ToList();
        _logger.LogDebug("Retrieved {Count} labels for card {CardId}", labels.Count, cardId);
        return _mapper.Map<List<LabelResponse>>(labels);
    }

    public async Task<CardLabelResponse> AddLabelToCard(int cardId, AddCardLabelDto dto)
    {
        CardLabel newCardLabel = new CardLabel(cardId, dto.LabelId);
        await _cardLabelRepository.CreateAsync(newCardLabel);
        await _unitOfWork.CommitAsync();

        var cardLabel = await _cardLabelRepository
            .GetCardLabelByIdAsync(newCardLabel.CardId, newCardLabel.LabelId);

        _logger.LogInformation("Label {LabelId} added to card {CardId}", dto.LabelId, cardId);
        return _mapper.Map<CardLabelResponse>(cardLabel);
    }

    public async Task<Boolean> RemoveLabelFromCard(int cardId, int labelId)
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
}