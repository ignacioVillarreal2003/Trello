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

    public async Task<OutputCommentDetailsDto?> AddComment(int cardId, AddCommentDto dto, int uid)
    {
        try
        {
            Comment comment = new Comment(dto.Text, cardId, dto.AuthorId);
            await _commentRepository.AddComment(comment);

            _logger.LogInformation("Comment added to card {CardId}", cardId);
            return _mapper.Map<OutputCommentDetailsDto>(comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding comment to card {CardId}", cardId);
            throw;
        }
    }

    public async Task<OutputCommentDetailsDto?> UpdateComment(int commentId, UpdateCommentDto dto, int uid)
    {
        try
        {
            Comment? comment = await _commentRepository.GetCommentById(commentId);
            if (comment == null)
            {
                _logger.LogWarning("Comment {CommentId} not found for update", commentId);
                return null;
            }

            if (!string.IsNullOrEmpty(dto.Text))
            {
                comment.Text = dto.Text;
            }

            await _commentRepository.UpdateComment(comment);
            
            _logger.LogInformation("Comment {CommentId} updated", commentId);
            return _mapper.Map<OutputCommentDetailsDto>(comment);
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

            await _commentRepository.DeleteComment(comment);

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