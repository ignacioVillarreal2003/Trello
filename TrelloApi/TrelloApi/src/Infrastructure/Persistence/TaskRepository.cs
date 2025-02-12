using Microsoft.EntityFrameworkCore;
using TrelloApi.app;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Task;
using Task = TrelloApi.Domain.Task.Task;

namespace TrelloApi.Infrastructure.Persistence;

public class TaskRepository: Repository<Task>, ITaskRepository
{
    private readonly ILogger<TaskRepository> _logger;
    
    public TaskRepository(TrelloContext context, ILogger<TaskRepository> logger) : base(context)
    {
        _logger = logger;
    }
    
    public async Task<Task?> GetTaskById(int taskId)
    {
        try
        {
            Task? task = await Context.Tasks
                .FirstOrDefaultAsync(t => t.Id.Equals(taskId));
            
            _logger.LogDebug("Task {TaskId} retrieval attempt completed", taskId);
            return task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving task {TaskId}", taskId);
            throw;
        }
    }

    public async Task<List<Task>> GetTasksByListId(int listId)
    {
        try
        {
            List<Task> tasks = await Context.Tasks
                .Where(t => t.ListId == listId)
                .ToListAsync();

            _logger.LogDebug("Retrieved {Count} task for list {ListId}", tasks.Count, listId);
            return tasks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving tasks for list {ListId}", listId);
            throw;
        }
    }

    public async Task<Task?> AddTask(Task task)
    {
        try
        {
            await Context.Tasks.AddAsync(task);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("Task {TaskId} added successfully", task.Id);
            return task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error adding task");
            throw;
        }
    }

    public async Task<Task?> UpdateTask(Task task)
    {
        try
        {
            Context.Tasks.Update(task);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("Task {TaskId} updated", task.Id);
            return task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error updating task {TaskId}", task.Id);
            throw;
        }
    }

    public async Task<Task?> DeleteTask(Task task)
    {
        try
        {
            Context.Tasks.Remove(task);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("Task {TaskId} deleted", task.Id);
            return task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error deleting task {TaskId}", task.Id);
            throw;
        }
    }
}