using AutoMapper;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.DTOs.Comment;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Application.Services;

public class CommentService: BaseService, ICommentService
{
    private readonly ILogger<CommentService> _logger;
    private readonly ICommentRepository _commentRepository;

    public CommentService(IMapper mapper, 
        IUnitOfWork unitOfWork, 
        ILogger<CommentService> logger, 
        ICommentRepository commentRepository) 
        : base(mapper, unitOfWork)
    {
        _logger = logger;
        _commentRepository = commentRepository;
    }
    
    public async Task<CommentResponse?> GetCommentById(int commentId)
    {
        try
        {
            Comment? comment = await _commentRepository.GetAsync(c => c.Id.Equals(commentId));
            if (comment == null)
            {
                _logger.LogWarning("Comment {CommentId} not found", commentId);
                return null;
            }

            _logger.LogDebug("Comment {CommentId} retrieved", commentId);
            return _mapper.Map<CommentResponse>(comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving comment {CommentId}", commentId);
            throw;
        }
    }
    
    public async Task<List<CommentResponse>> GetCommentsByCardId(int cardId)
    {
        try
        {
            List<Comment> comments = (await _commentRepository.GetListAsync(c => c.CardId.Equals(cardId))).ToList();
            _logger.LogDebug("Retrieved {Count} comments for card {CardId}", comments.Count, cardId);
            return _mapper.Map<List<CommentResponse>>(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving comments for card {CardId}", cardId);
            throw;
        }
    }

    public async Task<CommentResponse> AddComment(int cardId, AddCommentDto dto, int userId)
    {
        try
        {
            Comment comment = new Comment(dto.Text, cardId, userId);
            await _commentRepository.CreateAsync(comment);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Comment added to card {CardId}", cardId);
            return _mapper.Map<CommentResponse>(comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding comment to card {CardId}", cardId);
            throw;
        }
    }

    public async Task<CommentResponse?> UpdateComment(int commentId, UpdateCommentDto dto)
    {
        try
        {
            Comment? comment = await _commentRepository.GetAsync(c => c.Id.Equals(commentId));
            if (comment == null)
            {
                _logger.LogWarning("Comment {CommentId} not found for update", commentId);
                return null;
            }

            if (!string.IsNullOrEmpty(dto.Text))
            {
                comment.Text = dto.Text;
            }

            await _commentRepository.UpdateAsync(comment);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Comment {CommentId} updated", commentId);
            return _mapper.Map<CommentResponse>(comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating comment {CommentId}", commentId);
            throw;
        }
    }

    public async Task<Boolean> DeleteComment(int commentId)
    {
        try
        {
            Comment? comment = await _commentRepository.GetAsync(c => c.Id.Equals(commentId));
            if (comment == null)
            {
                _logger.LogWarning("Comment {CommentId} not found for deletion", commentId);
                return false;
            }

            await _commentRepository.DeleteAsync(comment);
            await _unitOfWork.CommitAsync();

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