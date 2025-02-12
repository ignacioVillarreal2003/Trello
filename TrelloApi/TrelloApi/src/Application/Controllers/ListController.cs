using Microsoft.AspNetCore.Mvc;
using TrelloApi.Application.Filters;
using TrelloApi.Application.Services;
using TrelloApi.Domain.Entities.List;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Application.Controllers;

[ApiController]
[Route("[controller]")]
[RequireAuthentication]
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
            OutputListDto? list = await _listService.GetListById(listId, UserId);
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
            List<OutputListDto> lists = await _listService.GetListsByBoardId(boardId, UserId);
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
    public async Task<IActionResult> AddList(int boardId, [FromBody] AddListDto addListDto)
    {
        try
        {
            OutputListDto? list = await _listService.AddList(addListDto, boardId, UserId);
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
    public async Task<IActionResult> UpdateList(int listId, [FromBody] UpdateListDto updateListDto)
    {
        try
        {
            OutputListDto? list = await _listService.UpdateList(listId, updateListDto, UserId);
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
            OutputListDto? list = await _listService.DeleteList(listId, UserId);
            if (list == null)
            {
                _logger.LogDebug("List {ListId} not found for deletion", listId);
                return NotFound(new { message = "List not found." });
            }
            
            _logger.LogInformation("List {ListId} deleted", listId);
            return Ok(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting list {ListId}", listId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}