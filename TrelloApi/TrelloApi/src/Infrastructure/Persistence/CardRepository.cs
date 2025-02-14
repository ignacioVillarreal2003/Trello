using Microsoft.EntityFrameworkCore;
using TrelloApi.app;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;

namespace TrelloApi.Infrastructure.Persistence;

public class CardRepository: Repository<Card>, ICardRepository
{
    private readonly ILogger<CardRepository> _logger;
    
    public CardRepository(TrelloContext context, ILogger<CardRepository> logger) : base(context)
    {
        _logger = logger;
    }
    
    public async Task<Card?> GetCardById(int cardId)
    {
        try
        {
            Card? card = await Context.Cards
                .FirstOrDefaultAsync(t => t.Id.Equals(cardId));
            
            _logger.LogDebug("Card {CardId} retrieval attempt completed", cardId);
            return card;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving card {CardId}", cardId);
            throw;
        }
    }

    public async Task<List<Card>> GetCardsByListId(int listId)
    {
        try
        {
            List<Card> cards = await Context.Cards
                .Where(t => t.ListId == listId)
                .ToListAsync();

            _logger.LogDebug("Retrieved {Count} cards for list {ListId}", cards.Count, listId);
            return cards;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving cards for list {ListId}", listId);
            throw;
        }
    }

    public async Task<Card?> AddCard(Card card)
    {
        try
        {
            await Context.Cards.AddAsync(card);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("Card {CardId} added successfully", card.Id);
            return card;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error adding card");
            throw;
        }
    }

    public async Task<Card?> UpdateCard(Card card)
    {
        try
        {
            Context.Cards.Update(card);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("Card {CardId} updated", card.Id);
            return card;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error updating card {CardId}", card.Id);
            throw;
        }
    }

    public async Task<Card?> DeleteCard(Card card)
    {
        try
        {
            Context.Cards.Remove(card);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("Card {CardId} deleted", card.Id);
            return card;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error deleting card {CardId}", card.Id);
            throw;
        }
    }
}