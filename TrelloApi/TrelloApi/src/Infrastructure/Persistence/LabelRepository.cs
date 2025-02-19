using Microsoft.EntityFrameworkCore;
using TrelloApi.app;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;

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
                .FirstOrDefaultAsync(l => l.Id == labelId);

            _logger.LogDebug("Label {LabelId} retrieval attempt completed", labelId);
            return label;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving label {LabelId}", labelId);
            throw;
        }
    }

    public async Task<List<Label>> GetLabelsByBoardId(int boardId)
    {
        try
        {
            List<Label> labels = await Context.Labels
                .Where(l => l.BoardId == boardId)
                .ToListAsync();

            _logger.LogDebug("Retrieved {Count} labels for board {BoardId}", labels.Count, boardId);
            return labels;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving labels for board {BoardId}", boardId);
            throw;
        }
    }
    
    public async Task AddLabel(Label label)
    {
        try
        {
            await Context.Labels.AddAsync(label);
            await Context.SaveChangesAsync();
            _logger.LogDebug("Label {LabelId} added.", label.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error adding label.");
            throw;
        }
    }

    public async Task UpdateLabel(Label label)
    {
        try
        {
            Context.Labels.Update(label);
            await Context.SaveChangesAsync();
            _logger.LogDebug("Label {LabelId} updated", label.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error updating label {LabelId}", label.Id);
            throw;
        }
    }

    public async Task DeleteLabel(Label label)
    {
        try
        {
            Context.Labels.Remove(label);
            await Context.SaveChangesAsync();
            _logger.LogDebug("Label {LabelId} deleted", label.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error deleting label {LabelId}", label.Id);
            throw;
        }
    }
}