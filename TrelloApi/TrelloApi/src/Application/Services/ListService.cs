using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.DTOs.List;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Application.Services;

public class ListService: BaseService, IListService
{
    private readonly ILogger<ListService> _logger;
    private readonly IListRepository _listRepository;
    
    public ListService(IMapper mapper, 
        IUnitOfWork unitOfWork,
        ILogger<ListService> logger, 
        IListRepository listRepository)
        : base(mapper, unitOfWork)
    {
        _logger = logger;
        _listRepository = listRepository;
    }
    
    public async Task<ListResponse?> GetListById(int listId)
    {
        List? list = await _listRepository.GetAsync(l => l.Id.Equals(listId));
        if (list == null)
        {
            _logger.LogWarning("List {ListId} not found", listId);
            return null;
        }

        _logger.LogDebug("List {ListId} retrieved", listId);
        return _mapper.Map<ListResponse>(list);
    }
    
    public async Task<List<ListResponse>> GetListsByBoardId(int boardId)
    {
        List<List> lists = (await _listRepository.GetListAsync(l => l.BoardId.Equals(boardId))).ToList();
        _logger.LogDebug("Retrieved {Count} lists for board {BoardId}", lists.Count, boardId);
        return _mapper.Map<List<ListResponse>>(lists);
    }

    public async Task<ListResponse> AddList(int boardId, AddListDto dto)
    {
        List list = new List(dto.Title, boardId, dto.Position);
            
        await _listRepository.CreateAsync(list);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("List added to board {BoardId}", boardId);
        return _mapper.Map<ListResponse>(list);
    }

    public async Task<ListResponse?> UpdateList(int listId, UpdateListDto dto)
    {
        List? list = await _listRepository.GetAsync(l => l.Id.Equals(listId));
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
            
        await _listRepository.UpdateAsync(list);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("List {ListId} updated", listId);
        return _mapper.Map<ListResponse>(list);
    }

    public async Task<Boolean> DeleteList(int listId)
    {
        List? list = await _listRepository.GetAsync(l => l.Id.Equals(listId));
        if (list == null)
        {
            _logger.LogWarning("List {ListId} not found for deletion", listId);
            return false;
        }
            
        await _listRepository.DeleteAsync(list);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("List {ListId} deleted", listId);
        return true;
    }
    
    public async Task<ListResponse> GetListByIdToAccess(int listId)
    {
        List list = await _listRepository.GetListByIdToAccessAsync(listId);
        _logger.LogInformation("List {ListId} access success", listId);
        return _mapper.Map<ListResponse>(list);
    }
}