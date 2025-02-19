using Microsoft.EntityFrameworkCore;
using TrelloApi.app;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;

namespace TrelloApi.Infrastructure.Persistence;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(TrelloContext context, ILogger<UserRepository> logger) : base(context)
    {
        _logger = logger;
    }

    public async Task<User?> GetUserById(int userId)
    {
        try
        {
            User? user = await Context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            
            _logger.LogDebug("User {UserId} retrieval attempt completed", userId);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving user {UserId}", userId);
            throw;
        }
    }
    
    public async Task<User?> GetUserByEmail(string email)
    {
        try
        {
            User? user = await Context.Users.FirstOrDefaultAsync(u => u.Email == email);
            
            _logger.LogDebug("User {Email} retrieval attempt completed", email);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving user {Email}", email);
            throw;
        }
    }
    
    public async Task<List<User>> GetUsers()
    {
        try
        {
            List<User> users = await Context.Users.ToListAsync();
            
            _logger.LogDebug("Retrieved {Count} users", users.Count);
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving users");
            throw;
        }
    }

    public async Task<List<User>> GetUsersByUsername(string username)
    {
        try
        {
            List<User> users = await Context.Users
                .Where(u => u.Username.ToLower().StartsWith(username.ToLower()))
                .ToListAsync();
            
            _logger.LogDebug("Retrieved {Count} users for username {Username}", users.Count, username);
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving users for username {Username}", username);
            throw;
        }
    }

    public async Task<List<User>> GetUsersByCardId(int cardId)
    {
        try
        {
            List<User> users = await Context.Users
                .Join(Context.UserCards, 
                    user => user.Id, 
                    userCard => userCard.UserId, 
                    (user, userCard) => new { user, userCard })
                .Where(uc => uc.userCard.CardId.Equals(cardId))
                .Select(ut => ut.user)
                .ToListAsync();
            
            _logger.LogDebug("Retrieved {Count} users for card {CardId}", users.Count, cardId);
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving users for card {CardId}", cardId);
            throw;
        }
    }

    public async Task AddUser(User user)
    {
        try
        {
            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync();
            _logger.LogDebug("User {UserId} added successfully", user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error adding user");
            throw;
        }
    }

    public async Task UpdateUser(User user)
    {
        try
        {
            Context.Users.Update(user);
            await Context.SaveChangesAsync();
            _logger.LogDebug("User {UserId} updated", user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error updating user {UserId}", user.Id);
            throw;
        }
    }

    public async Task DeleteUser(User user)
    {
        try
        {
            Context.Users.Remove(user);
            await Context.SaveChangesAsync();
            _logger.LogDebug("User {UserId} deleted", user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error deleting user {UserId}", user.Id);
            throw;
        }
    }
}
