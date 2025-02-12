using Microsoft.EntityFrameworkCore;
using TrelloApi.app;
using TrelloApi.Domain.Entities.TaskLabel;
using TrelloApi.Domain.Interfaces.Repositories;

namespace TrelloApi.Infrastructure.Persistence;

public class TaskLabelRepository: Repository<TaskLabel>, ITaskLabelRepository
{
    private readonly ILogger<TaskLabelRepository> _logger;
    
    public TaskLabelRepository(TrelloContext context, ILogger<TaskLabelRepository> logger) : base(context)
    {
        _logger = logger;
    }

    public async Task<TaskLabel?> GetTaskLabelById(int taskId, int labelId)
    {
        try
        {
            TaskLabel? taskLabel = await Context.TaskLabels
                .FirstOrDefaultAsync(tl => tl.TaskId.Equals(taskId) && tl.LabelId.Equals(labelId));

            _logger.LogDebug("Label {LabelId} for task {TaskId} retrieval attempt completed", labelId, taskId);
            return taskLabel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving label {labelId} for task {taskId}", labelId, taskId);
            throw;
        }
    }

    public async Task<TaskLabel?> AddTaskLabel(TaskLabel taskLabel)
    {
        try
        {
            await Context.TaskLabels.AddAsync(taskLabel);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("Label {LabelId} added to task {TaskId}", taskLabel.LabelId, taskLabel.TaskId);
            return taskLabel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error adding label {LabelId} to task {TaskId}", taskLabel.LabelId, taskLabel.TaskId);
            throw;
        }
    }

    public async Task<TaskLabel?> DeleteTaskLabel(TaskLabel taskLabel)
    {
        try
        {
            Context.TaskLabels.Remove(taskLabel);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("Label {LabelId} for task {TaskId} deleted", taskLabel.LabelId, taskLabel.TaskId);
            return taskLabel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error deleting label {LabelId} for task {TaskId}", taskLabel.LabelId, taskLabel.TaskId);
            throw;
        }
    }
}