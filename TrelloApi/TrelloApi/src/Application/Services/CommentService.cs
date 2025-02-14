using AutoMapper;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Entities;
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
    
    public async Task<OutputCommentDetailsDto?> GetCommentById(int commentId, int uid)
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
            return _mapper.Map<OutputCommentDetailsDto>(comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving comment {CommentId}", commentId);
            throw;
        }
    }
    
    public async Task<List<OutputCommentDetailsDto>> GetCommentsByCardId(int cardId, int uid)
    {
        try
        {
            List<Comment> comments = await _commentRepository.GetCommentsByCardId(cardId);
            _logger.LogDebug("Retrieved {Count} comments for card {CardId}", comments.Count, cardId);
            return _mapper.Map<List<OutputCommentDetailsDto>>(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving comments for card {CardId}", cardId);
            throw;
        }
    }

    public async Task<OutputCommentDetailsDto?> AddComment(int cardId, AddCommentDto addCommentDto, int uid)
    {
        try
        {
            Comment comment = new Comment(addCommentDto.Text, cardId, addCommentDto.AuthorId);
            Comment? newComment = await _commentRepository.AddComment(comment);
            if (newComment == null)
            {
                _logger.LogError("Failed to add comment to card {CardId}", cardId);
                return null;
            }

            _logger.LogInformation("Comment added to card {CardId}", cardId);
            return _mapper.Map<OutputCommentDetailsDto>(newComment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding comment to card {CardId}", cardId);
            throw;
        }
    }

    public async Task<OutputCommentDetailsDto?> UpdateComment(int commentId, UpdateCommentDto updateCommentDto, int uid)
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
            return _mapper.Map<OutputCommentDetailsDto>(updatedComment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating comment {CommentId}", commentId);
            throw;
        }
    }

    public async Task<Boolean> DeleteComment(int commentId, int uid)
    {
        try
        {
            Comment? comment = await _commentRepository.GetCommentById(commentId);
            if (comment == null)
            {
                _logger.LogWarning("Comment {CommentId} not found for deletion", commentId);
                return false;
            }

            Comment? deletedComment = await _commentRepository.DeleteComment(comment);
            if (deletedComment == null)
            {
                _logger.LogError("Failed to delete comment {CommentId}", commentId);
                return false;
            }

            _logger.LogInformation("Comment {CommentId} deleted", commentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting comment {CommentId}", commentId);
            throw;
        }
    }
}