using Microsoft.EntityFrameworkCore;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Generics;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Infrastructure.Persistence.Repositories;

public class CommentRepository : GenericRepository<Comment>, ICommentRepository
{
    public CommentRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    
    public async Task<Comment> GetCommentByIdToAccessAsync(int commentId)
    {
        return await Context.Comments
            .Include(c => c.Card.List.Board)
            .FirstOrDefaultAsync(c => c.Id.Equals(commentId));
    }
}