using Microsoft.EntityFrameworkCore;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Generics;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Infrastructure.Persistence.Repositories;

public class CardLabelRepository: GenericRepository<CardLabel>, ICardLabelRepository
{
    public CardLabelRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    
    public async Task<IEnumerable<Label>> GetLabelsByCardIdAsync(int cardId)
    {
        return await Context.Labels
            .Join(Context.CardLabels,
                label => label.Id,
                cardLabel => cardLabel.LabelId,
                (label, cardLabel) => new {label, cardLabel})
            .Where(cl => cl.cardLabel.CardId.Equals(cardId))
            .Select(cl => cl.label)
            .ToListAsync();
    }
}