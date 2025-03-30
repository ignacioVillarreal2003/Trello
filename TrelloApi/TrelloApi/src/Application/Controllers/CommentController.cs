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
        var authorize = await TryAuthorizeCommentAsync(commentId);
        if (authorize != null)
        {
            return authorize;
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

    [HttpGet("card/{cardId:int}")]
    public async Task<IActionResult> GetCommentsByCardId(int cardId)
    {
        var authorize = await TryAuthorizeCardAsync(cardId);
        if (authorize != null)
        {
            return authorize;
        }
            
        List<CommentResponse> comments = await _commentService.GetCommentsByCardId(cardId);
        _logger.LogDebug("Retrieved {Count} comments for card {CardId}", comments.Count, cardId);
        return Ok(comments);
    }

    [HttpPost("card/{cardId:int}")]
    public async Task<IActionResult> AddComment(int cardId, [FromBody] AddCommentDto dto)
    {
        var authorize = await TryAuthorizeCardAsync(cardId);
        if (authorize != null)
        {
            return authorize;
        }
            
        CommentResponse comment = await _commentService.AddComment(cardId, dto, UserId);
            
        await _hubContext.Clients.Group(BoardId.ToString())
            .SendAsync("CommentCreated", comment);
            
        _logger.LogInformation("Comment added to card {CardId}", cardId);
        return CreatedAtAction(nameof(GetCommentById), new { commentId = comment.Id }, comment);
    }

    [HttpPut("{commentId:int}")]
    public async Task<IActionResult> UpdateComment(int commentId, [FromBody] UpdateCommentDto dto)
    {
        var authorize = await TryAuthorizeCommentAsync(commentId);
        if (authorize != null)
        {
            return authorize;
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

    [HttpDelete("{commentId:int}")]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        var authorize = await TryAuthorizeCommentAsync(commentId);
        if (authorize != null)
        {
            return authorize;
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
    
    private async Task<IActionResult?> TryAuthorizeCardAsync (int cardId)
    {
        CardResponse? card = await _cardService.GetCardByIdToAccess(cardId);
        if (card == null)
        {
            _logger.LogDebug("Card {CardId} not found for access.", cardId);
            return NotFound(new { message = "Card not found." });
        }
        
        var authResult = await _authorizationService.AuthorizeAsync(User, card.List.BoardId, "BoardAccessPolicy");
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        return null;
    }
    
    private async Task<IActionResult?> TryAuthorizeCommentAsync (int commentId)
    {
        CommentResponse? comment = await _commentService.GetCommentByIdToAccess(commentId);
        if (comment == null)
        {
            _logger.LogDebug("Comment {CommentId} not found for access.", commentId);
            return NotFound(new { message = "Comment not found." });
        }
        
        var authResult = await _authorizationService.AuthorizeAsync(User, comment.Card.List.BoardId, "BoardAccessPolicy");
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        return null;
    }
}