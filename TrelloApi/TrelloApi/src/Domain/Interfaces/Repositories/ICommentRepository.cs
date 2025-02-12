namespace TrelloApi.Domain.Interfaces.Repositories;

public interface ICommentRepository
{
    Task<Comment.Comment?> GetCommentById(int commentId);
    Task<List<Comment.Comment>> GetCommentsByTaskId(int taskId);
    Task<Comment.Comment?> AddComment(Comment.Comment comment);
    Task<Comment.Comment?> UpdateComment(Comment.Comment comment);
    Task<Comment.Comment?> DeleteComment(Comment.Comment comment);
}