using Microsoft.EntityFrameworkCore;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Generics;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Infrastructure.Persistence.Repositories;

public class UserCardRepository: GenericRepository<UserCard>, IUserCardRepository
{
    public UserCardRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    
    public async Task<IEnumerable<User>> GetUsersByCardIdAsync(int cardId)
    {
        return await Context.UserCards
            .Join(Context.Users,
                userCard => userCard.UserId,
                user => user.Id,
                (userCard, user) => new { userCard, user })
            .Where(uc => uc.userCard.CardId.Equals(cardId))
            .Select(uc => uc.user)
            .ToListAsync();
    }
}