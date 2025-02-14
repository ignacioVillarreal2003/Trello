using AutoMapper;
using TrelloApi.Domain.Constants;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Application.Services;

public class CardService: BaseService, ICardService
{
    private readonly ILogger<CardService> _logger;
    private readonly ICardRepository _cardRepository;

    public CardService(IMapper mapper, IBoardAuthorizationService boardAuthorizationService, ILogger<CardService> logger, ICardRepository cardRepository): base(mapper, boardAuthorizationService)
    {
        _logger = logger;
        _cardRepository = cardRepository;
    }
    
    public async Task<OutputCardDetailsDto?> GetCardById(int cardId, int uid)
    {
        try
        {
            Card? card = await _cardRepository.GetCardById(cardId);
            if (card == null)
            {
                _logger.LogWarning("Card {CardId} not found", cardId);
                return null;
            }

            _logger.LogDebug("Card {CardId} retrieved", cardId);
            return _mapper.Map<OutputCardDetailsDto>(card);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving card {CardId}", cardId);
            throw;
        }
    }

    public async Task<List<OutputCardListDto>> GetCardsByListId(int listId, int uid)
    {
        try
        {
            List<Card> cards = await _cardRepository.GetCardsByListId(listId);
            _logger.LogDebug("Retrieved {Count} cards for list {ListId}", cards.Count, listId);
            return _mapper.Map<List<OutputCardListDto>>(cards);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cards for list {ListId}", listId);
            throw;
        }
    }

    public async Task<OutputCardDetailsDto?> AddCard(int listId, AddCardDto dto, int uid)
    {
        try
        {
            Card card = new Card(dto.Title, dto.Description, listId, dto.Priority);
            Card? newCard = await _cardRepository.AddCard(card);
            if (newCard == null)
            {
                _logger.LogError("Failed to add card to list {ListId}", listId);
                return null;
            }

            _logger.LogInformation("Card added to list {ListId}", listId);
            return _mapper.Map<OutputCardDetailsDto>(newCard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding card to list {ListId}", listId);
            throw;
        }
    }

    public async Task<OutputCardDetailsDto?> UpdateCard(int cardId, UpdateCardDto dto, int uid)
    {
        try
        {
            Card? card = await _cardRepository.GetCardById(cardId);
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
            if (!string.IsNullOrEmpty(dto.Priority) && PriorityValues.PrioritiesAllowed.Contains(dto.Priority))
            {
                card.Priority = dto.Priority;
            }
            card.UpdatedAt = DateTime.UtcNow;

            Card? updatedCard = await _cardRepository.UpdateCard(card);
            if (updatedCard == null)
            {
                _logger.LogError("Failed to update card {CardId}", cardId);
                return null;
            }
            
            _logger.LogInformation("Card {CardId} updated", cardId);
            return _mapper.Map<OutputCardDetailsDto>(updatedCard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating card {CardId}", cardId);
            throw;
        }
    }

    public async Task<Boolean> DeleteCard(int cardId, int uid)
    {
        try
        {
            Card? card = await _cardRepository.GetCardById(cardId);
            if (card == null)
            {
                _logger.LogWarning("Card {CardId} not found for deletion", cardId);
                return false;
            }

            Card? deletedCard = await _cardRepository.DeleteCard(card);
            if (deletedCard == null)
            {
                _logger.LogError("Failed to delete card {CardId}", cardId);
                return false;
            }

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