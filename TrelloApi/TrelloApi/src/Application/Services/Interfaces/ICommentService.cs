using TrelloApi.Domain.DTOs.Comment;

namespace TrelloApi.Application.Services.Interfaces;

public interface ICommentService
{
    Task<CommentResponse?> GetCommentById(int commentId);
    Task<List<CommentResponse>> GetCommentsByCardId(int cardId);
    Task<CommentResponse?> AddComment(int taskId, AddCommentDto dto, int userId);
    Task<CommentResponse?> UpdateComment(int commentId, UpdateCommentDto dto);
    Task<Boolean> DeleteComment(int commentId);
}