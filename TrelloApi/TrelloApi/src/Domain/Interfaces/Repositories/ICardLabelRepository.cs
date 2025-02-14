using TrelloApi.Domain.Entities;

namespace TrelloApi.Domain.Interfaces.Repositories;

public interface ICardLabelRepository
{
    Task<CardLabel?> GetCardLabelById(int cardId, int labelId);
    Task<List<Label>> GetLabelsByCardId(int cardId);
    Task<CardLabel?> AddCardLabel(CardLabel cardLabel);
    Task<CardLabel?> DeleteCardLabel(CardLabel cardLabel);
}