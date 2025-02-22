using Microsoft.EntityFrameworkCore;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Generics;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Infrastructure.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

    public async Task<IEnumerable<User>> GetUsersByUsernameAsync(string username)
    {
        return await Context.Users
            .Where(u => u.Username.ToLower().StartsWith(username.ToLower()))
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetUsersByCardIdAsync(int cardId)
    {
        return await Context.Users
            .Join(Context.UserCards, 
                user => user.Id, 
                userCard => userCard.UserId, 
                (user, userCard) => new { user, userCard })
            .Where(uc => uc.userCard.CardId.Equals(cardId))
            .Select(ut => ut.user)
            .ToListAsync();
    }
}
