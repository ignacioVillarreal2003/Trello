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
    
    public async Task<List<User>> GetUsersByCardId(int cardId)
    {
        try
        {
            List<User> users = await Context.UserCards
                .Join(Context.Users,
                    userCard => userCard.UserId,
                    user => user.Id,
                    (userCard, user) => new { userCard, user })
                .Where(uc => uc.userCard.CardId.Equals(cardId))
                .Select(uc => uc.user)
                .ToListAsync();

            _logger.LogDebug("Users for card {CardId} retrieval attempt completed", cardId);
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving users for card {CardId}", cardId);
            throw;
        }
    }

    public async Task AddUserCard(UserCard userCard)
    {
        try
        {
            await Context.UserCards.AddAsync(userCard);
            await Context.SaveChangesAsync();
            _logger.LogDebug("Card {CardId} added to user {UserId}", userCard.CardId, userCard.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error adding card {CardId} for user {UserId}", userCard.CardId, userCard.UserId);
            throw;
        }
    }

    public async Task DeleteUserCard(UserCard userCard)
    {
        try
        {
            Context.UserCards.Remove(userCard);
            await Context.SaveChangesAsync();
            _logger.LogDebug("Card {CardId} for user {UserId} deleted", userCard.CardId, userCard.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error deleting card {CardId} for user {UserId}", userCard.CardId, userCard.UserId);
            throw;
        }
    }
}