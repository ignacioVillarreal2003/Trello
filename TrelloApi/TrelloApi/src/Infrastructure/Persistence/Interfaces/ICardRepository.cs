using TrelloApi.Domain.Entities;

namespace TrelloApi.Infrastructure.Persistence.Interfaces;

public interface ICardRepository: IGenericRepository<Card>
{
    Task<Card> GetCardByIdToAccessAsync(int cardId);
}