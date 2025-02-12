using AutoMapper;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.Comment;
using TrelloApi.Domain.Comment.DTO;
using TrelloApi.Domain.Entities.Comment;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Application.Services;

public class CommentService: BaseService, ICommentService
{
    private readonly ILogger<CommentService> _logger;
    private readonly ICommentRepository _commentRepository;

    public CommentService(IMapper mapper, IBoardAuthorizationService boardAuthorizationService, ILogger<CommentService> logger, ICommentRepository commentRepository) : base(mapper, boardAuthorizationService)
    {
        _logger = logger;
        _commentRepository = commentRepository;
    }
    
    public async Task<OutputCommentDto?> GetCommentById(int commentId, int userId)
    {
        try
        {
            Comment? comment = await _commentRepository.GetCommentById(commentId);
            if (comment == null)
            {
                _logger.LogWarning("Comment {CommentId} not found", commentId);
                return null;
            }

            _logger.LogDebug("Comment {CommentId} retrieved", commentId);
            return _mapper.Map<OutputCommentDto>(comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving comment {CommentId}", commentId);
            throw;
        }
    }
    
    public async Task<List<OutputCommentDto>> GetCommentsByTaskId(int taskId, int userId)
    {
        try
        {
            List<Comment> comments = await _commentRepository.GetCommentsByTaskId(taskId);
            _logger.LogDebug("Retrieved {Count} comments for task {TaskId}", comments.Count, taskId);
            return _mapper.Map<List<OutputCommentDto>>(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving comments for task {TaskId}", taskId);
            throw;
        }
    }

    public async Task<OutputCommentDto?> AddComment(int taskId, AddCommentDto addCommentDto, int userId)
    {
        try
        {
            Comment comment = new Comment(addCommentDto.Text, taskId, addCommentDto.AuthorId);
            Comment? newComment = await _commentRepository.AddComment(comment);
            if (newComment == null)
            {
                _logger.LogError("Failed to add comment to task {TaskId}", taskId);
                return null;
            }

            _logger.LogInformation("Comment added to task {TaskId}", taskId);
            return _mapper.Map<OutputCommentDto>(newComment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding comment to task {TaskId}", taskId);
            throw;
        }
    }

    public async Task<OutputCommentDto?> UpdateComment(int commentId, UpdateCommentDto updateCommentDto, int userId)
    {
        try
        {
            Comment? comment = await _commentRepository.GetCommentById(commentId);
            if (comment == null)
            {
                _logger.LogWarning("Comment {CommentId} not found for update", commentId);
                return null;
            }

            if (!string.IsNullOrEmpty(updateCommentDto.Text))
            {
                comment.Text = updateCommentDto.Text;
            }

            Comment? updatedComment = await _commentRepository.UpdateComment(comment);
            if (updatedComment == null)
            {
                _logger.LogError("Failed to update comment {CommentId}", commentId);
                return null;
            }
            
            _logger.LogInformation("Comment {CommentId} updated", commentId);
            return _mapper.Map<OutputCommentDto>(updatedComment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating comment {CommentId}", commentId);
            throw;
        }
    }

    public async Task<OutputCommentDto?> DeleteComment(int commentId, int userId)
    {
        try
        {
            Comment? comment = await _commentRepository.GetCommentById(commentId);
            if (comment == null)
            {
                _logger.LogWarning("Comment {CommentId} not found for deletion", commentId);
                return null;
            }

            Comment? deletedComment = await _commentRepository.DeleteComment(comment);
            if (deletedComment == null)
            {
                _logger.LogError("Failed to delete comment {CommentId}", commentId);
                return null;
            }

            _logger.LogInformation("Comment {CommentId} deleted", commentId);
            return _mapper.Map<OutputCommentDto>(deletedComment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting comment {CommentId}", commentId);
            throw;
        }
    }
}