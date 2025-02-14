using TrelloApi.Domain.Entities;

namespace TrelloApi.Domain.Interfaces.Repositories;

public interface ICommentRepository
{
    Task<Comment?> GetCommentById(int commentId);
    Task<List<Comment>> GetCommentsByCardId(int cardId);
    Task<Comment?> AddComment(Comment comment);
    Task<Comment?> UpdateComment(Comment comment);
    Task<Comment?> DeleteComment(Comment comment);
}