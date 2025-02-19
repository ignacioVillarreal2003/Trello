using TrelloApi.Domain.Entities;

namespace TrelloApi.Domain.Interfaces.Repositories;

public interface ICardLabelRepository
{
    Task<CardLabel?> GetCardLabelById(int cardId, int labelId);
    Task<List<Label>> GetLabelsByCardId(int cardId);
    Task AddCardLabel(CardLabel cardLabel);
    Task DeleteCardLabel(CardLabel cardLabel);
}