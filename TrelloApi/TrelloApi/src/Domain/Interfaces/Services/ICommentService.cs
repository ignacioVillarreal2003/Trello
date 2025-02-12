using TrelloApi.Domain.Comment.DTO;
using TrelloApi.Domain.Entities.Comment;

namespace TrelloApi.Domain.Interfaces.Services;

public interface ICommentService
{
    Task<OutputCommentDto?> GetCommentById(int commentId, int userId);
    Task<List<OutputCommentDto>> GetCommentsByTaskId(int taskId, int userId);
    Task<OutputCommentDto?> AddComment(int taskId, AddCommentDto addCommentDto, int userId);
    Task<OutputCommentDto?> UpdateComment(int commentId, UpdateCommentDto updateCommentDto, int userId);
    Task<OutputCommentDto?> DeleteComment(int commentId, int userId);
}