using Microsoft.EntityFrameworkCore;
using TrelloApi.app;
using TrelloApi.Domain.Entities.User;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.User;

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

    public async Task<List<User>> GetUsersByBoardId(int boardId)
    {
        try
        {
            List<User> users = await Context.Users
                .Join(Context.UserBoards, 
                    user => user.Id, 
                    userBoard => userBoard.UserId, 
                    (user, userBoard) => new { user, userBoard })
                .Where(ub => ub.userBoard.BoardId.Equals(boardId))
                .Select(ub => ub.user)
                .ToListAsync();
            
            _logger.LogDebug("Retrieved {Count} users for board {BoardId}", users.Count, boardId);
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving users for board {BoardId}", boardId);
            throw;
        }
    }

    public async Task<List<User>> GetUsersByTaskId(int taskId)
    {
        try
        {
            List<User> users = await Context.Users
                .Join(Context.UserTasks, 
                    user => user.Id, 
                    userTask => userTask.UserId, 
                    (user, userTask) => new { user, userTask })
                .Where(ut => ut.userTask.TaskId.Equals(taskId))
                .Select(ut => ut.user)
                .ToListAsync();
            
            _logger.LogDebug("Retrieved {Count} users for task {TaskId}", users.Count, taskId);
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving users for task {TaskId}", taskId);
            throw;
        }
    }

    public async Task<User?> AddUser(User user)
    {
        try
        {
            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("User {UserId} added successfully", user.Id);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error adding user");
            throw;
        }
    }

    public async Task<User?> UpdateUser(User user)
    {
        try
        {
            Context.Users.Update(user);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("User {UserId} updated", user.Id);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error updating user {UserId}", user.Id);
            throw;
        }
    }

    public async Task<User?> DeleteUser(User user)
    {
        try
        {
            Context.Users.Remove(user);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("User {UserId} deleted", user.Id);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error deleting user {UserId}", user.Id);
            throw;
        }
    }
}
