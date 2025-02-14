using TrelloApi.Domain.Entities;

namespace TrelloApi.Domain.Interfaces.Repositories;

public interface ICardRepository
{
    Task<Card?> GetCardById(int cardId);
    Task<List<Card>> GetCardsByListId(int listId);
    Task<Card?> AddCard(Card card);
    Task<Card?> UpdateCard(Card card);
    Task<Card?> DeleteCard(Card card);
}