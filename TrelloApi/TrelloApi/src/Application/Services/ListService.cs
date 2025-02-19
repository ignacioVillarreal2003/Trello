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
    
    public async Task<OutputListDetailsDto?> GetListById(int listId, int uid)
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
            return _mapper.Map<OutputListDetailsDto>(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving list {ListId}", listId);
            throw;
        }
    }
    
    public async Task<List<OutputListDetailsDto>> GetListsByBoardId(int boardId, int uid)
    {
        try
        {
            List<List> lists = await _listRepository.GetListsByBoardId(boardId);
            _logger.LogDebug("Retrieved {Count} lists for board {BoardId}", lists.Count, boardId);
            return _mapper.Map<List<OutputListDetailsDto>>(lists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving lists for board {BoardId}", boardId);
            throw;
        }
    }

    public async Task<OutputListDetailsDto?> AddList(int boardId, AddListDto dto, int uid)
    {
        try
        {
            List list = new List(dto.Title, boardId, dto.Position);
            
            await _listRepository.AddList(list);

            _logger.LogInformation("List added to board {BoardId}", boardId);
            return _mapper.Map<OutputListDetailsDto>(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding list to board {BoardId}", boardId);
            throw;
        }
    }

    public async Task<OutputListDetailsDto?> UpdateList(int listId, UpdateListDto dto, int uid)
    {
        try
        {
            List? list = await _listRepository.GetListById(listId);
            if (list == null)
            {
                _logger.LogWarning("List {ListId} not found for update", listId);
                return null;
            }

            if (!string.IsNullOrEmpty(dto.Title))
            {
                list.Title = dto.Title;
            }
            if (dto.Position != null)
            {
                list.Position = dto.Position.Value;
            }

            list.UpdatedAt = DateTime.UtcNow;
            
            await _listRepository.UpdateList(list);
            
            _logger.LogInformation("List {ListId} updated", listId);
            return _mapper.Map<OutputListDetailsDto>(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating list {ListId}", listId);
            throw;
        }
    }

    public async Task<Boolean> DeleteList(int listId, int uid)
    {
        try
        {
            List? list = await _listRepository.GetListById(listId);
            if (list == null)
            {
                _logger.LogWarning("List {ListId} not found for deletion", listId);
                return false;
            }
            
            await _listRepository.DeleteList(list);
            
            _logger.LogInformation("List {ListId} deleted", listId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting list {ListId}", listId);
            throw;
        }
    }
}