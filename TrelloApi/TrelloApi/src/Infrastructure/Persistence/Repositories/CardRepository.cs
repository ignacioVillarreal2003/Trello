using Microsoft.EntityFrameworkCore;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Generics;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Infrastructure.Persistence.Repositories;

public class CardRepository: GenericRepository<Card>, ICardRepository
{
    public CardRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    
    public async Task<Card> GetCardByIdToAccessAsync(int cardId)
    {
        return await Context.Cards
            .Include(c => c.List.Board)
            .FirstOrDefaultAsync(c => c.Id.Equals(cardId));
    }

    public async Task<Card?> GetCardByIdCompleteAsync(int cardId)
    {
        return await Context.Cards
            .Include(c => c.Comments)
            .Include(c => c.CardLabels)
            .ThenInclude(cl => cl.Label)
            .Include(c => c.UserCards)
            .ThenInclude(uc => uc.User)
            .FirstOrDefaultAsync(c => c.Id.Equals(cardId));
    }
}