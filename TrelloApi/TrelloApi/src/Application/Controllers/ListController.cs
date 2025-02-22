using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TrelloApi.Application.Services.Interfaces;
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
    
    public ListController(ILogger<ListController> logger, IListService listService)
    {
        _logger = logger;
        _listService = listService;
    }
    
    [HttpGet("{listId:int}")]
    public async Task<IActionResult> GetListById(int listId)
    {
        try
        {
            ListResponse? list = await _listService.GetListById(listId);
            if (list == null)
            {
                _logger.LogDebug("List {ListId} not found", listId);
                return NotFound(new { message = "List not found." });
            }

            _logger.LogDebug("List {ListId} retrieved", listId);
            return Ok(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving list {ListId}", listId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpGet("board/{boardId:int}")]
    public async Task<IActionResult> GetListsByBoardId(int boardId)
    {
        try
        {
            List<ListResponse> lists = await _listService.GetListsByBoardId(boardId);
            _logger.LogDebug("Retrieved {Count} lists for board {BoardId}", lists.Count, boardId);
            return Ok(lists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving lists for board {BoardId}", boardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpPost("board/{boardId:int}")]
    public async Task<IActionResult> AddList(int boardId, [FromBody] AddListDto dto)
    {
        try
        {
            ListResponse? list = await _listService.AddList(boardId, dto);
            if (list == null)
            {
                _logger.LogError("Failed to add list to board {BoardId}", boardId);
                return BadRequest(new { message = "Failed to add list." });
            }
            
            _logger.LogInformation("List added to board {BoardId}", boardId);
            return CreatedAtAction(nameof(GetListById), new { listId = list.Id }, list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding list to board {BoardId}", boardId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpPut("{listId:int}")]
    public async Task<IActionResult> UpdateList(int listId, [FromBody] UpdateListDto dto)
    {
        try
        {
            ListResponse? list = await _listService.UpdateList(listId, dto);
            if (list == null)
            {
                _logger.LogDebug("List {ListId} not found for update", listId);
                return NotFound(new { message = "List not found." });
            }

            _logger.LogInformation("List {ListId} updated", listId);
            return Ok(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating list {ListId}", listId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpDelete("{listId:int}")]
    public async Task<IActionResult> DeleteList(int listId)
    {
        try
        {
            Boolean isDeleted = await _listService.DeleteList(listId);
            if (!isDeleted)
            {
                _logger.LogDebug("List {ListId} not found for deletion", listId);
                return NotFound(new { message = "List not found." });
            }
            
            _logger.LogInformation("List {ListId} deleted", listId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting list {ListId}", listId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}