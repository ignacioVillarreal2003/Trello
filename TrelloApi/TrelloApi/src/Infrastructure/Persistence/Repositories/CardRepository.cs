using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Generics;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Infrastructure.Persistence.Repositories;

public class CardRepository: GenericRepository<Card>, ICardRepository
{
    public CardRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }
}