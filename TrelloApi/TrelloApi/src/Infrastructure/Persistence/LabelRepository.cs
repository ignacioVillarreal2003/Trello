using Microsoft.EntityFrameworkCore;
using TrelloApi.app;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Label;

namespace TrelloApi.Infrastructure.Persistence;

public class LabelRepository: Repository<Label>, ILabelRepository
{
    private readonly ILogger<LabelRepository> _logger;
    
    public LabelRepository(TrelloContext context, ILogger<LabelRepository> logger) : base(context)
    {
        _logger = logger;
    }
    
    public async Task<Label?> GetLabelById(int labelId)
    {
        try
        {
            Label? label = await Context.Labels
                .FirstOrDefaultAsync(l => l.Id.Equals(labelId));

            _logger.LogDebug("Label {LabelId} retrieval attempt completed", labelId);
            return label;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving label {LabelId}", labelId);
            throw;
        }
    }

    public async Task<List<Label>> GetLabelsByTaskId(int taskId)
    {
        try
        {
            List<Label> labels = await Context.Labels
                .Join(Context.TaskLabels,
                    label => label.Id,
                    taskLabel => taskLabel.LabelId,
                    (label, taskLabel) => new {label, taskLabel})
                .Where(tl => tl.taskLabel.TaskId.Equals(taskId))
                .Select(tl => tl.label)
                .ToListAsync();
            
            _logger.LogDebug("Retrieved {Count} labels for task {TaskId}", labels.Count, taskId);
            return labels;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving labels for task {TaskId}", taskId);
            throw;
        }
    }

    public async Task<Label?> AddLabel(Label label)
    {
        try
        {
            await Context.Labels.AddAsync(label);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("Label {LabelId} added.", label.Id);
            return label;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error adding label.");
            throw;
        }
    }

    public async Task<Label?> UpdateLabel(Label label)
    {
        try
        {
            Context.Labels.Update(label);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("Label {LabelId} updated", label.Id);
            return label;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error updating label {LabelId}", label.Id);
            throw;
        }
    }

    public async Task<Label?> DeleteLabel(Label label)
    {
        try
        {
            Context.Labels.Remove(label);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("Label {LabelId} deleted", label.Id);
            return label;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error deleting label {LabelId}", label.Id);
            throw;
        }
    }
}