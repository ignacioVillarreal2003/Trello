using Microsoft.EntityFrameworkCore;
using TrelloApi.app;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;

namespace TrelloApi.Infrastructure.Persistence;

public class CardLabelRepository: Repository<CardLabel>, ICardLabelRepository
{
    private readonly ILogger<CardLabelRepository> _logger;
    
    public CardLabelRepository(TrelloContext context, ILogger<CardLabelRepository> logger) : base(context)
    {
        _logger = logger;
    }

    public async Task<CardLabel?> GetCardLabelById(int cardId, int labelId)
    {
        try
        {
            CardLabel? cardLabel = await Context.CardLabels
                .FirstOrDefaultAsync(cl => cl.CardId.Equals(cardId) && cl.LabelId.Equals(labelId));

            _logger.LogDebug("Label {LabelId} for card {CardId} retrieval attempt completed", labelId, cardId);
            return cardLabel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving label {labelId} for card {CardId}", labelId, cardId);
            throw;
        }
    }
    
    public async Task<List<Label>> GetLabelsByCardId(int cardId)
    {
        try
        {
            List<Label> labels = await Context.Labels
                .Join(Context.CardLabels,
                    label => label.Id,
                    cardLabel => cardLabel.LabelId,
                    (label, cardLabel) => new {label, cardLabel})
                .Where(cl => cl.cardLabel.CardId.Equals(cardId))
                .Select(cl => cl.label)
                .ToListAsync();
            
            _logger.LogDebug("Retrieved {Count} labels for card {CardId}", labels.Count, cardId);
            return labels;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving labels for card {CardId}", cardId);
            throw;
        }
    }

    public async Task AddCardLabel(CardLabel cardLabel)
    {
        try
        {
            await Context.CardLabels.AddAsync(cardLabel);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("Label {LabelId} added to card {CardId}", cardLabel.LabelId, cardLabel.CardId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error adding label {LabelId} to card {CardId}", cardLabel.LabelId, cardLabel.CardId);
            throw;
        }
    }

    public async Task DeleteCardLabel(CardLabel cardLabel)
    {
        try
        {
            Context.CardLabels.Remove(cardLabel);
            await Context.SaveChangesAsync();
            _logger.LogDebug("Label {LabelId} for card {CardId} deleted", cardLabel.LabelId, cardLabel.CardId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error deleting label {LabelId} for card {CardId}", cardLabel.LabelId, cardLabel.CardId);
            throw;
        }
    }
}