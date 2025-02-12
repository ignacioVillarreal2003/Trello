using Microsoft.AspNetCore.Mvc;
using TrelloApi.Application.Filters;
using TrelloApi.Application.Services;
using TrelloApi.Domain.Comment;
using TrelloApi.Domain.Comment.DTO;
using TrelloApi.Domain.Entities.Comment;
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
            OutputCommentDto? comment = await _commentService.GetCommentById(commentId, UserId);
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

    [HttpGet("task/{taskId:int}")]
    public async Task<IActionResult> GetCommentsByTaskId(int taskId)
    {
        try
        {
            List<OutputCommentDto> comments = await _commentService.GetCommentsByTaskId(taskId, UserId);
            _logger.LogDebug("Retrieved {Count} comments for task {TaskId}", comments.Count, taskId);
            return Ok(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving comments for task {TaskId}", taskId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpPost("task/{taskId:int}")]
    public async Task<IActionResult> AddComment(int taskId, [FromBody] AddCommentDto addCommentDto)
    {
        try
        {
            OutputCommentDto? comment = await _commentService.AddComment(taskId, addCommentDto, UserId);
            if (comment == null)
            {
                _logger.LogError("Failed to add comment to task {TaskId}", taskId);
                return BadRequest(new { message = "Failed to add comment." });
            }

            _logger.LogInformation("Comment added to task {TaskId}", taskId);
            return CreatedAtAction(nameof(GetCommentById), new { commentId = comment.Id }, comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding comment to task {TaskId}", taskId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpPut("{commentId:int}")]
    public async Task<IActionResult> UpdateComment(int commentId, [FromBody] UpdateCommentDto updateCommentDto)
    {
        try
        {
            OutputCommentDto? comment = await _commentService.UpdateComment(commentId, updateCommentDto, UserId);
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
            OutputCommentDto? comment = await _commentService.DeleteComment(commentId, UserId);
            if (comment == null)
            {
                _logger.LogDebug("Comment {CommentId} not found for deletion", commentId);
                return NotFound(new { message = "Comment not found." });
            }

            _logger.LogInformation("Comment {CommentId} deleted", commentId);
            return Ok(comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting comment {CommentId}", commentId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}