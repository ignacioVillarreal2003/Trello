using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Application.Services;

public class ListService: BaseService, IListService
{
    private readonly ILogger<ListService> _logger;
    private readonly IListRepository _listRepository;
    
    public ListService(IMapper mapper, IBoardAuthorizationService boardAuthorizationService,  ILogger<ListService> logger, IListRepository listRepository): base(mapper, boardAuthorizationService)
    {
        _logger = logger;
        _listRepository = listRepository;
    }
    
    public async Task<OutputListDto?> GetListById(int listId, int userId)
    {
        try
        {
            List? list = await _listRepository.GetListById(listId);
            if (list == null)
            {
                _logger.LogWarning("List {ListId} not found", listId);
                return null;
            }

            _logger.LogDebug("List {ListId} retrieved", listId);
            return _mapper.Map<OutputListDto>(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving list {ListId}", listId);
            throw;
        }
    }
    
    public async Task<List<OutputListDto>> GetListsByBoardId(int boardId, int userId)
    {
        try
        {
            List<List> lists = await _listRepository.GetListsByBoardId(boardId);
            _logger.LogDebug("Retrieved {Count} lists for board {BoardId}", lists.Count, boardId);
            return _mapper.Map<List<OutputListDto>>(lists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving lists for board {BoardId}", boardId);
            throw;
        }
    }

    public async Task<OutputListDto?> AddList(AddListDto addListDto, int boardId, int userId)
    {
        try
        {
            List list = new List(addListDto.Title, boardId);
            List? newList = await _listRepository.AddList(list);
            if (newList == null)
            {
                _logger.LogError("Failed to add list to board {BoardId}", boardId);
                return null;
            }

            _logger.LogInformation("List added to board {BoardId}", boardId);
            return _mapper.Map<OutputListDto>(newList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding list to board {BoardId}", boardId);
            throw;
        }
    }

    public async Task<OutputListDto?> UpdateList(int listId, UpdateListDto updateListDto, int userId)
    {
        try
        {
            List? list = await _listRepository.GetListById(listId);
            if (list == null)
            {
                _logger.LogWarning("List {ListId} not found for update", listId);
                return null;
            }

            if (!string.IsNullOrEmpty(updateListDto.Title))
            {
                list.Title = updateListDto.Title;
            }
            
            List? updatedList = await _listRepository.UpdateList(list);
            if (updatedList == null)
            {
                _logger.LogError("Failed to update list {ListId}", listId);
                return null;
            }
            
            _logger.LogInformation("List {ListId} updated", listId);
            return _mapper.Map<OutputListDto>(updatedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating list {ListId}", listId);
            throw;
        }
    }

    public async Task<OutputListDto?> DeleteList(int listId, int userId)
    {
        try
        {
            List? list = await _listRepository.GetListById(listId);
            if (list == null)
            {
                _logger.LogWarning("List {ListId} not found for deletion", listId);
                return null;
            }
            
            List? deletedList = await _listRepository.DeleteList(list);
            if (deletedList == null)
            {
                _logger.LogError("Failed to delete list {ListId}", listId);
                return null;
            }
            
            _logger.LogInformation("List {ListId} deleted", listId);
            return _mapper.Map<OutputListDto>(deletedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting list {ListId}", listId);
            throw;
        }
    }
}