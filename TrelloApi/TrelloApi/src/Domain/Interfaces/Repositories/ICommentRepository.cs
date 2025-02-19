using TrelloApi.Domain.Entities;

namespace TrelloApi.Domain.Interfaces.Repositories;

public interface ICommentRepository
{
    Task<Comment?> GetCommentById(int commentId);
    Task<List<Comment>> GetCommentsByCardId(int cardId);
    Task AddComment(Comment comment);
    Task UpdateComment(Comment comment);
    Task DeleteComment(Comment comment);
}