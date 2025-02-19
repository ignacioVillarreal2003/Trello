using TrelloApi.Domain.Entities;

namespace TrelloApi.Domain.Interfaces.Repositories;

public interface ICardRepository
{
    Task<Card?> GetCardById(int cardId);
    Task<List<Card>> GetCardsByListId(int listId);
    Task AddCard(Card card);
    Task UpdateCard(Card card);
    Task DeleteCard(Card card);
}