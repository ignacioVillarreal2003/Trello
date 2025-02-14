using Microsoft.EntityFrameworkCore;
using TrelloApi.app;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;

namespace TrelloApi.Infrastructure.Persistence;

public class UserCardRepository: Repository<UserCard>, IUserCardRepository
{
    private readonly ILogger<UserCardRepository> _logger;
    public UserCardRepository(TrelloContext context, ILogger<UserCardRepository> logger) : base(context)
    {
        _logger = logger;
    }

    public async Task<UserCard?> GetUserCardById(int userId, int cardId)
    {
        try
        {
            UserCard? userCard = await Context.UserCards
                .FirstOrDefaultAsync(uc => uc.UserId.Equals(userId) && uc.CardId.Equals(cardId));

            _logger.LogDebug("Card {CardId} for user {UserId} retrieval attempt completed", cardId, userId);
            return userCard;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving card {CardId} for user {UserId}", cardId, userId);
            throw;
        }
    }

    public async Task<UserCard?> AddUserCard(UserCard userCard)
    {
        try
        {
            await Context.UserCards.AddAsync(userCard);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("Card {CardId} added to user {UserId}", userCard.CardId, userCard.UserId);
            return userCard;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error adding card {CardId} for user {UserId}", userCard.CardId, userCard.UserId);
            throw;
        }
    }

    public async Task<UserCard?> DeleteUserCard(UserCard userCard)
    {
        try
        {
            Context.UserCards.Remove(userCard);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("Card {CardId} for user {UserId} deleted", userCard.CardId, userCard.UserId);
            return userCard;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error deleting card {CardId} for user {UserId}", userCard.CardId, userCard.UserId);
            throw;
        }
    }
}