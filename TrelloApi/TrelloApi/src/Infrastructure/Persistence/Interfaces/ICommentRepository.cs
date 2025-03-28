using TrelloApi.Domain.Entities;

namespace TrelloApi.Infrastructure.Persistence.Interfaces;

public interface ICommentRepository: IGenericRepository<Comment>
{
    Task<Comment> GetCommentByIdToAccessAsync(int commentId);
}