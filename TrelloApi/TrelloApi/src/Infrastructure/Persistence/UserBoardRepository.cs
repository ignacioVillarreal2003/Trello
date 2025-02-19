using Microsoft.EntityFrameworkCore;
using TrelloApi.app;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;

namespace TrelloApi.Infrastructure.Persistence;

public class UserBoardRepository: Repository<UserBoard>, IUserBoardRepository
{
    private readonly ILogger<UserBoardRepository> _logger;
    
    public UserBoardRepository(TrelloContext context, ILogger<UserBoardRepository> logger) : base(context)
    {
        _logger = logger;
    }

    public async Task<UserBoard?> GetUserBoardById(int userId, int boardId)
    {
        try
        {
            UserBoard? userBoard = await Context.UserBoards
                .FirstOrDefaultAsync(ub => ub.UserId.Equals(userId) && ub.BoardId.Equals(boardId));

            _logger.LogDebug("User {UserId} for board {BoardId} retrieval attempt completed", userId, boardId);
            return userBoard;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving user {UserId} for board {BoardId}", userId, boardId);
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

            _logger.LogDebug("Users for board {BoardId} retrieval attempt completed", boardId);
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving users for board {BoardId}", boardId);
            throw;
        }
    }

    public async Task AddUserBoard(UserBoard userBoard)
    {
        try
        {
            await Context.UserBoards.AddAsync(userBoard);
            await Context.SaveChangesAsync();
            _logger.LogDebug("User {UserId} added to board {BoardId}", userBoard.UserId, userBoard.BoardId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error adding user {UserId} to board {BoardId}", userBoard.UserId, userBoard.BoardId);
            throw;
        }
    }

    public async Task DeleteUserBoard(UserBoard userBoard)
    {
        try
        {
            Context.UserBoards.Remove(userBoard);
            await Context.SaveChangesAsync();
            _logger.LogDebug("User {UserId} for board {BoardId} deleted", userBoard.UserId, userBoard.BoardId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error deleting user {UserId} for board {BoardId}", userBoard.UserId, userBoard.BoardId);
            throw;
        }
    }
}