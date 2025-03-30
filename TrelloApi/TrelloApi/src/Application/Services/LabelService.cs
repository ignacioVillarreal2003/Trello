using AutoMapper;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.Constants;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.DTOs.Label;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Application.Services;

public class LabelService: BaseService, ILabelService
{
    private readonly ILogger<LabelService> _logger;
    private readonly ILabelRepository _labelRepository;

    public LabelService(IMapper mapper, 
        IUnitOfWork unitOfWork, 
        ILogger<LabelService> logger, 
        ILabelRepository labelRepository) 
        : base(mapper, unitOfWork)
    {
        _logger = logger;
        _labelRepository = labelRepository;
    }
    
    public async Task<LabelResponse?> GetLabelById(int labelId)
    {
        Label? label = await _labelRepository.GetAsync(l => l.Id.Equals(labelId));
        if (label == null)
        {
            _logger.LogWarning("Label {LabelId} not found", labelId);
            return null;
        }

        _logger.LogDebug("Label {LabelId} retrieved", labelId);
        return _mapper.Map<LabelResponse>(label);
    }
    
    public async Task<List<LabelResponse>> GetLabelsByBoardId(int boardId)
    {
        List<Label> labels = (await _labelRepository.GetListAsync(l => l.BoardId.Equals(boardId))).ToList();
        _logger.LogDebug("Retrieved {Count} labels for board {BoardId}", labels.Count, boardId);
        return _mapper.Map<List<LabelResponse>>(labels);
    }

    public async Task<LabelResponse> AddLabel(int boardId, AddLabelDto dto)
    {
        Label label = new Label(dto.Title, dto.Color, boardId);
        await _labelRepository.CreateAsync(label);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("Label added to board {BoardId}", boardId);
        return _mapper.Map<LabelResponse>(label);
    }

    public async Task<LabelResponse?> UpdateLabel(int labelId, UpdateLabelDto dto)
    {
        Label? label = await _labelRepository.GetAsync(l => l.Id.Equals(labelId));
        if (label == null)
        {
            _logger.LogWarning("Label {LabelId} not found for update", labelId);
            return null;
        }

        if (!string.IsNullOrEmpty(dto.Color))
        {
            label.Color = dto.Color;
        }
        if (!string.IsNullOrEmpty(dto.Title))
        {
            label.Title = dto.Title;
        }

        await _labelRepository.UpdateAsync(label);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("Label {LabelId} updated", labelId);
        return _mapper.Map<LabelResponse>(label);
    }

    public async Task<bool> DeleteLabel(int labelId)
    {
        Label? label = await _labelRepository.GetAsync(l => l.Id.Equals(labelId));
        if (label == null)
        {
            _logger.LogWarning("Label {LabelId} not found for deletion", labelId);
            return false;
        }

        await _labelRepository.DeleteAsync(label);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("Label {LabelId} deleted", labelId);
        return true;
    }
    
    public async Task<LabelResponse> GetLabelByIdToAccess(int labelId)
    {
        Label label = await _labelRepository.GetLabelByIdToAccessAsync(labelId);
        _logger.LogInformation("Label {LabelId} access success", labelId);
        return _mapper.Map<LabelResponse>(label);
    }
}