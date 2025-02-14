using Microsoft.AspNetCore.Mvc;
using TrelloApi.Application.Filters;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Application.Controllers;

[ApiController]
[Route("[controller]")]
[RequireAuthentication]
public class CommentController: BaseController
{
    private readonly ILogger<CommentController> _logger;
    private readonly ICommentService _commentService;

    public CommentController(ILogger<CommentController> logger, ICommentService commentService)
    {
        _logger = logger;
        _commentService = commentService;
    }

    [HttpGet("{commentId:int}")]
    public async Task<IActionResult> GetCommentById(int commentId)
    {
        try
        {
            OutputCommentDetailsDto? comment = await _commentService.GetCommentById(commentId, UserId);
            if (comment == null)
            {
                _logger.LogDebug("Comment {CommentId} not found", commentId);
                return NotFound(new { message = "Comment not found." });
            }

            _logger.LogDebug("Comment {CommentId} retrieved", commentId);
            return Ok(comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving comment {CommentId}", commentId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpGet("card/{cardId:int}")]
    public async Task<IActionResult> GetCommentsByCardId(int cardId)
    {
        try
        {
            List<OutputCommentDetailsDto> comments = await _commentService.GetCommentsByCardId(cardId, UserId);
            _logger.LogDebug("Retrieved {Count} comments for card {CardId}", comments.Count, cardId);
            return Ok(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving comments for card {CardId}", cardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpPost("card/{cardId:int}")]
    public async Task<IActionResult> AddComment(int cardId, [FromBody] AddCommentDto dto)
    {
        try
        {
            OutputCommentDetailsDto? comment = await _commentService.AddComment(cardId, dto, UserId);
            if (comment == null)
            {
                _logger.LogError("Failed to add comment to card {CardId}", cardId);
                return BadRequest(new { message = "Failed to add comment." });
            }

            _logger.LogInformation("Comment added to card {CardId}", cardId);
            return CreatedAtAction(nameof(GetCommentById), new { commentId = comment.Id }, comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding comment to card {CardId}", cardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpPut("{commentId:int}")]
    public async Task<IActionResult> UpdateComment(int commentId, [FromBody] UpdateCommentDto dto)
    {
        try
        {
            OutputCommentDetailsDto? comment = await _commentService.UpdateComment(commentId, dto, UserId);
            if (comment == null)
            {
                _logger.LogDebug("Comment {CommentId} not found for update", commentId);
                return NotFound(new { message = "Comment not found." });
            }
            
            _logger.LogInformation("Comment {CommentId} updated", commentId);
            return Ok(comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating comment {CommentId}", commentId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpDelete("{commentId:int}")]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        try
        {
            Boolean isDeleted = await _commentService.DeleteComment(commentId, UserId);
            if (!isDeleted)
            {
                _logger.LogDebug("Comment {CommentId} not found for deletion", commentId);
                return NotFound(new { message = "Comment not found." });
            }

            _logger.LogInformation("Comment {CommentId} deleted", commentId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting comment {CommentId}", commentId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}