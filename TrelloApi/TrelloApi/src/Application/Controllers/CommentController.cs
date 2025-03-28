using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using TrelloApi.Application.Hub;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.Card;
using TrelloApi.Domain.DTOs.Comment;

namespace TrelloApi.Application.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
[EnableRateLimiting("fixed")]
public class CommentController: BaseController
{
    private readonly ILogger<CommentController> _logger;
    private readonly ICommentService _commentService;
    private readonly IHubContext<BoardHub> _hubContext;
    private readonly IAuthorizationService _authorizationService;
    private readonly ICardService _cardService;

    public CommentController(ILogger<CommentController> logger, ICommentService commentService, ICardService cardService, IHubContext<BoardHub> hubContext, IAuthorizationService authorizationService)
    {
        _logger = logger;
        _commentService = commentService;
        _hubContext = hubContext;
        _authorizationService = authorizationService;
        _cardService = cardService;
    }

    [HttpGet("{commentId:int}")]
    public async Task<IActionResult> GetCommentById(int commentId)
    {
        try
        {
            CommentResponse? commentToAccess = await _commentService.GetCommentByIdToAccess(commentId);
            if (commentToAccess == null)
            {
                _logger.LogDebug("Comment {CommentId} not found for got.", commentId);
                return NotFound(new { message = "Comment not found." });
            }
        
            var authResult = await _authorizationService.AuthorizeAsync(User, commentToAccess.Card.List.BoardId, "BoardAccessPolicy");
            if (!authResult.Succeeded)
            {
                return Forbid();
            }
            
            CommentResponse? comment = await _commentService.GetCommentById(commentId);
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
            CardResponse? cardToAccess = await _cardService.GetCardByIdToAccess(cardId);
            if (cardToAccess == null)
            {
                _logger.LogDebug("Card {CardId} not found for got.", cardId);
                return NotFound(new { message = "Card not found." });
            }
        
            var authResult = await _authorizationService.AuthorizeAsync(User, cardToAccess.List.BoardId, "BoardAccessPolicy");
            if (!authResult.Succeeded)
            {
                return Forbid();
            }
            
            List<CommentResponse> comments = await _commentService.GetCommentsByCardId(cardId);
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
            CardResponse? cardToAccess = await _cardService.GetCardByIdToAccess(cardId);
            if (cardToAccess == null)
            {
                _logger.LogDebug("Card {CardId} not found for got.", cardId);
                return NotFound(new { message = "Card not found." });
            }
        
            var authResult = await _authorizationService.AuthorizeAsync(User, cardToAccess.List.BoardId, "BoardAccessPolicy");
            if (!authResult.Succeeded)
            {
                return Forbid();
            }
            
            CommentResponse comment = await _commentService.AddComment(cardId, dto, UserId);
            
            await _hubContext.Clients.Group(BoardId.ToString())
                .SendAsync("CommentCreated", comment);
            
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
            CommentResponse? commentToAccess = await _commentService.GetCommentByIdToAccess(commentId);
            if (commentToAccess == null)
            {
                _logger.LogDebug("Comment {CommentId} not found for got.", commentId);
                return NotFound(new { message = "Comment not found." });
            }
        
            var authResult = await _authorizationService.AuthorizeAsync(User, commentToAccess.Card.List.BoardId, "BoardAccessPolicy");
            if (!authResult.Succeeded)
            {
                return Forbid();
            }
            
            CommentResponse? comment = await _commentService.UpdateComment(commentId, dto);
            if (comment == null)
            {
                _logger.LogDebug("Comment {CommentId} not found for update", commentId);
                return NotFound(new { message = "Comment not found." });
            }
            
            await _hubContext.Clients.Group(BoardId.ToString())
                .SendAsync("CommentUpdated", comment);
            
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
            CommentResponse? commentToAccess = await _commentService.GetCommentByIdToAccess(commentId);
            if (commentToAccess == null)
            {
                _logger.LogDebug("Comment {CommentId} not found for got.", commentId);
                return NotFound(new { message = "Comment not found." });
            }
        
            var authResult = await _authorizationService.AuthorizeAsync(User, commentToAccess.Card.List.BoardId, "BoardAccessPolicy");
            if (!authResult.Succeeded)
            {
                return Forbid();
            }
            
            Boolean isDeleted = await _commentService.DeleteComment(commentId);
            if (!isDeleted)
            {
                _logger.LogDebug("Comment {CommentId} not found for deletion", commentId);
                return NotFound(new { message = "Comment not found." });
            }

            await _hubContext.Clients.Group(BoardId.ToString())
                .SendAsync("CommentDeleted", commentId);
            
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