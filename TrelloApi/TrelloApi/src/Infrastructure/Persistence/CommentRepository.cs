using Microsoft.EntityFrameworkCore;
using TrelloApi.app;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;

namespace TrelloApi.Infrastructure.Persistence;

public class CommentRepository : Repository<Comment>, ICommentRepository
{
    private readonly ILogger<CommentRepository> _logger;
    
    public CommentRepository(TrelloContext context, ILogger<CommentRepository> logger) : base(context)
    {
        _logger = logger;
    }
    
    public async Task<Comment?> GetCommentById(int commentId)
    {
        try
        {
            Comment? comment = await Context.Comments
                .FirstOrDefaultAsync(c => c.Id.Equals(commentId));

            _logger.LogDebug("Comment {CommentId} retrieval attempt completed", commentId);
            return comment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving comment {CommentId}", commentId);
            throw;
        }
    }

    public async Task<List<Comment>> GetCommentsByCardId(int cardId)
    {
        try
        {
            List<Comment> comments = await Context.Comments
                .Where(c => c.CardId.Equals(cardId))
                .ToListAsync();

            _logger.LogDebug("Retrieved {Count} comments for card {CardId}", comments.Count, cardId);
            return comments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving comments for card {CardId}", cardId);
            throw;
        }
    }

    public async Task AddComment(Comment comment)
    {
        try
        {
            await Context.Comments.AddAsync(comment);
            await Context.SaveChangesAsync();
            _logger.LogDebug("Comment {CommentId} added to card {CardId}", comment.Id, comment.CardId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error adding comment to card {CardId}", comment.CardId);
            throw;
        }
    }

    public async Task UpdateComment(Comment comment)
    {
        try
        {
            Context.Comments.Update(comment);
            await Context.SaveChangesAsync();
            _logger.LogDebug("Comment {CommentId} updated", comment.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error updating comment {CommentId}", comment.Id);
            throw;
        }
    }

    public async Task DeleteComment(Comment comment)
    {
        try
        {
            Context.Comments.Remove(comment);
            await Context.SaveChangesAsync();
            _logger.LogDebug("Comment {CommentId} deleted", comment.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error deleting comment {CommentId}", comment.Id);
            throw;
        }
    }
}