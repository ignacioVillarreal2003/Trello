using TrelloApi.Domain.Entities;

namespace TrelloApi.Infrastructure.Persistence.Interfaces;

public interface ICardLabelRepository: IGenericRepository<CardLabel>
{
    Task<IEnumerable<Label>> GetLabelsByCardIdAsync(int cardId);
    Task<CardLabel> GetCardLabelByIdAsync(int cardId, int labelId);
    Task<CardLabel> GetCardLabelByIdToAccessAsync(int cardId, int labelId);
}