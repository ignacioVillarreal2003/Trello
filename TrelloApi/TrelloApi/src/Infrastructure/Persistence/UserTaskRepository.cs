using Microsoft.EntityFrameworkCore;
using TrelloApi.app;
using TrelloApi.Domain.Entities.UserTask;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.UserTask;

namespace TrelloApi.Infrastructure.Persistence;

public class UserTaskRepository: Repository<UserTask>, IUserTaskRepository
{
    private readonly ILogger<UserTaskRepository> _logger;
    public UserTaskRepository(TrelloContext context, ILogger<UserTaskRepository> logger) : base(context)
    {
        _logger = logger;
    }

    public async Task<UserTask?> GetUserTaskById(int userId, int taskId)
    {
        try
        {
            UserTask? userTask = await Context.UserTasks
                .FirstOrDefaultAsync(ut => ut.UserId.Equals(userId) && ut.TaskId.Equals(taskId));

            _logger.LogDebug("Task {TaskId} for user {UserId} retrieval attempt completed", taskId, userId);
            return userTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving task {TaskId} for user {UserId}", taskId, userId);
            throw;
        }
    }

    public async Task<UserTask?> AddUserTask(UserTask userTask)
    {
        try
        {
            await Context.UserTasks.AddAsync(userTask);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("Task {TaskId} added to user {UserId}", userTask.TaskId, userTask.UserId);
            return userTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error adding task {TaskId} for user {UserId}", userTask.TaskId, userTask.UserId);
            throw;
        }
    }

    public async Task<UserTask?> DeleteUserTask(UserTask userTask)
    {
        try
        {
            Context.UserTasks.Remove(userTask);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("Task {TaskId} for user {UserId} deleted", userTask.TaskId, userTask.UserId);
            return userTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error deleting task {TaskId} for user {UserId}", userTask.TaskId, userTask.UserId);
            throw;
        }
    }
}