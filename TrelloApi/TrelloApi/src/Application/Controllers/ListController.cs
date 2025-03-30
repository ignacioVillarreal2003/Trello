using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using TrelloApi.Application.Hub;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.Board;
using TrelloApi.Domain.DTOs.List;

namespace TrelloApi.Application.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
[EnableRateLimiting("fixed")]
public class ListController: BaseController
{
    private readonly ILogger<ListController> _logger;
    private readonly IListService _listService;
    private readonly IHubContext<BoardHub> _hubContext;
    private readonly IAuthorizationService _authorizationService;
    private readonly IBoardService _boardService;
    
    public ListController(ILogger<ListController> logger, IListService listService, IBoardService boardService, IHubContext<BoardHub> hubContext, IAuthorizationService authorizationService)
    {
        _logger = logger;
        _listService = listService;
        _hubContext = hubContext;
        _authorizationService = authorizationService;
        _boardService = boardService;
    }
    
    [HttpGet("{listId:int}")]
    public async Task<IActionResult> GetListById(int listId)
    {
        var authorize = await TryAuthorizeListAsync(listId);
        if (authorize != null)
        {
            return authorize;
        }
            
        ListResponse? list = await _listService.GetListById(listId);
        if (list == null)
        {
            _logger.LogDebug("List {ListId} not found", listId);
            return NotFound(new { message = "List not found." });
        }

        _logger.LogDebug("List {ListId} retrieved", listId);
        return Ok(list);
    }

    [HttpGet("board/{boardId:int}")]
    public async Task<IActionResult> GetListsByBoardId(int boardId)
    {
        var authorize = await TryAuthorizeBoardAsync(boardId);
        if (authorize != null)
        {
            return authorize;
        }
            
        List<ListResponse> lists = await _listService.GetListsByBoardId(boardId);
        _logger.LogDebug("Retrieved {Count} lists for board {BoardId}", lists.Count, boardId);
        return Ok(lists);
    }

    [HttpPost("board/{boardId:int}")]
    public async Task<IActionResult> AddList(int boardId, [FromBody] AddListDto dto)
    {
        var authorize = await TryAuthorizeBoardAsync(boardId);
        if (authorize != null)
        {
            return authorize;
        }
            
        ListResponse list = await _listService.AddList(boardId, dto);
            
        await _hubContext.Clients.Group(BoardId.ToString())
            .SendAsync("ListCreated", list);
            
        _logger.LogInformation("List added to board {BoardId}", boardId);
        return CreatedAtAction(nameof(GetListById), new { listId = list.Id }, list);
    }

    [HttpPut("{listId:int}")]
    public async Task<IActionResult> UpdateList(int listId, [FromBody] UpdateListDto dto)
    {
        var authorize = await TryAuthorizeListAsync(listId);
        if (authorize != null)
        {
            return authorize;
        }
            
        ListResponse? list = await _listService.UpdateList(listId, dto);
        if (list == null)
        {
            _logger.LogDebug("List {ListId} not found for update", listId);
            return NotFound(new { message = "List not found." });
        }
            
        await _hubContext.Clients.Group(BoardId.ToString())
            .SendAsync("ListUpdated", list);

        _logger.LogInformation("List {ListId} updated", listId);
        return Ok(list);
    }

    [HttpDelete("{listId:int}")]
    public async Task<IActionResult> DeleteList(int listId)
    {
        var authorize = await TryAuthorizeListAsync(listId);
        if (authorize != null)
        {
            return authorize;
        }
            
        Boolean isDeleted = await _listService.DeleteList(listId);
        if (!isDeleted)
        {
            _logger.LogDebug("List {ListId} not found for deletion", listId);
            return NotFound(new { message = "List not found." });
        }
            
        await _hubContext.Clients.Group(BoardId.ToString())
            .SendAsync("ListDeleted", listId);
            
        _logger.LogInformation("List {ListId} deleted", listId);
        return NoContent();
    }
    
    private async Task<IActionResult?> TryAuthorizeBoardAsync (int boardId)
    {
        BoardResponse? board = await _boardService.GetBoardByIdToAccess(boardId);
        if (board == null)
        {
            _logger.LogDebug("Board {BoardId} not found for access.", boardId);
            return NotFound(new { message = "Board not found." });
        }

        var authResult = await _authorizationService.AuthorizeAsync(User, board.Id, "BoardAccessPolicy");
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        return null;
    }
    
    private async Task<IActionResult?> TryAuthorizeListAsync (int listId)
    {
        ListResponse? list = await _listService.GetListByIdToAccess(listId);
        if (list == null)
        {
            _logger.LogDebug("List {ListId} not found for access.", listId);
            return NotFound(new { message = "List not found." });
        }
        
        var authResult = await _authorizationService.AuthorizeAsync(User, list.Board.Id, "BoardAccessPolicy");
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        return null;
    }
}