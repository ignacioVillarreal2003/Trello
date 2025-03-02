using AutoMapper;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.Constants;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.DTOs.Card;
using TrelloApi.Domain.DTOs.CardLabel;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Application.Services;

public class CardService: BaseService, ICardService
{
    private readonly ILogger<CardService> _logger;
    private readonly ICardRepository _cardRepository;

    public CardService(IMapper mapper, 
        IUnitOfWork unitOfWork, 
        ILogger<CardService> logger, 
        ICardRepository cardRepository)
        : base(mapper, unitOfWork)
    {
        _logger = logger;
        _cardRepository = cardRepository;
    }
    
    public async Task<CardResponse?> GetCardById(int cardId)
    {
        try
        {
            Card? card = await _cardRepository.GetAsync(c => c.Id.Equals(cardId));
            if (card == null)
            {
                _logger.LogWarning("Card {CardId} not found", cardId);
                return null;
            }

            _logger.LogDebug("Card {CardId} retrieved", cardId);
            return _mapper.Map<CardResponse>(card);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving card {CardId}", cardId);
            throw;
        }
    }

    public async Task<List<CardResponse>> GetCardsByListId(int listId)
    {
        try
        {
            List<Card> cards = (await _cardRepository.GetListAsync(c => c.ListId.Equals(listId))).ToList();
            _logger.LogDebug("Retrieved {Count} cards for list {ListId}", cards.Count, listId);
            return _mapper.Map<List<CardResponse>>(cards);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cards for list {ListId}", listId);
            throw;
        }
    }

    public async Task<CardResponse> AddCard(int listId, AddCardDto dto)
    {
        try
        {
            Card card = new Card(dto.Title, dto.Description, listId, dto.Priority);
            await _cardRepository.CreateAsync(card);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Card added to list {ListId}", listId);
            return _mapper.Map<CardResponse>(card);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding card to list {ListId}", listId);
            throw;
        }
    }

    public async Task<CardResponse?> UpdateCard(int cardId, UpdateCardDto dto)
    {
        try
        {
            Card? card = await _cardRepository.GetAsync(c => c.Id.Equals(cardId));
            if (card == null)
            {
                _logger.LogWarning("Task {cardId} not found for update", cardId);
                return null;
            }

            if (!string.IsNullOrEmpty(dto.Title))
            {
                card.Title = dto.Title;
            }
            if (!string.IsNullOrEmpty(dto.Description))
            {
                card.Description = dto.Description;
            }
            if (dto.ListId.HasValue)
            {
                card.ListId = dto.ListId.Value;
            }
            if (dto.DueDate.HasValue)
            {
                card.DueDate = dto.DueDate;
            }
            if (dto.IsCompleted != null)
            {
                card.IsCompleted = dto.IsCompleted.Value;
            }
            if (!string.IsNullOrEmpty(dto.Priority))
            {
                card.Priority = dto.Priority;
            }

            await _cardRepository.UpdateAsync(card);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Card {CardId} updated", cardId);
            return _mapper.Map<CardResponse>(card);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating card {CardId}", cardId);
            throw;
        }
    }

    public async Task<Boolean> DeleteCard(int cardId)
    {
        try
        {
            Card? card = await _cardRepository.GetAsync(c => c.Id.Equals(cardId));
            if (card == null)
            {
                _logger.LogWarning("Card {CardId} not found for deletion", cardId);
                return false;
            }

            await _cardRepository.DeleteAsync(card);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Card {CardId} deleted", cardId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting card {CardId}", cardId);
            throw;
        }
    }
}