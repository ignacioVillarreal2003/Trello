using TrelloApi.Domain.DTOs;

namespace TrelloApi.Domain.Interfaces.Services;

public interface ICommentService
{
    Task<OutputCommentDetailsDto?> GetCommentById(int commentId, int uid);
    Task<List<OutputCommentDetailsDto>> GetCommentsByCardId(int cardId, int uid);
    Task<OutputCommentDetailsDto?> AddComment(int taskId, AddCommentDto addCommentDto, int uid);
    Task<OutputCommentDetailsDto?> UpdateComment(int commentId, UpdateCommentDto updateCommentDto, int uid);
    Task<Boolean> DeleteComment(int commentId, int uid);
}