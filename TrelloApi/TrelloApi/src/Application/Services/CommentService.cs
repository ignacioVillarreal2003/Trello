using AutoMapper;
using TrelloApi.Application.Services.Interfaces;
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
        Comment? comment = await _commentRepository.GetAsync(c => c.Id.Equals(commentId));
        if (comment == null)
        {
            _logger.LogWarning("Comment {CommentId} not found", commentId);
            return null;
        }

        _logger.LogDebug("Comment {CommentId} retrieved", commentId);
        return _mapper.Map<CommentResponse>(comment);
    }
    
    public async Task<List<CommentResponse>> GetCommentsByCardId(int cardId)
    {
        List<Comment> comments = (await _commentRepository.GetListAsync(c => c.CardId.Equals(cardId))).ToList();
        _logger.LogDebug("Retrieved {Count} comments for card {CardId}", comments.Count, cardId);
        return _mapper.Map<List<CommentResponse>>(comments);
    }

    public async Task<CommentResponse> AddComment(int cardId, AddCommentDto dto, int userId)
    {
        Comment comment = new Comment(dto.Text, cardId, userId);
        await _commentRepository.CreateAsync(comment);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("Comment added to card {CardId}", cardId);
        return _mapper.Map<CommentResponse>(comment);
    }

    public async Task<CommentResponse?> UpdateComment(int commentId, UpdateCommentDto dto)
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

    public async Task<Boolean> DeleteComment(int commentId)
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
    
    public async Task<CommentResponse> GetCommentByIdToAccess(int commentId)
    {
        Comment comment = await _commentRepository.GetCommentByIdToAccessAsync(commentId);
        _logger.LogInformation("Comment {CommentId} access success", commentId);
        return _mapper.Map<CommentResponse>(comment);
    }
}