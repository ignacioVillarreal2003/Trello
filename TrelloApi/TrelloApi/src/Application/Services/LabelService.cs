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
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving label {LabelId}", labelId);
            throw;
        }
    }
    
    public async Task<List<LabelResponse>> GetLabelsByBoardId(int boardId)
    {
        try
        {
            List<Label> labels = (await _labelRepository.GetListAsync(l => l.BoardId.Equals(boardId))).ToList();
            _logger.LogDebug("Retrieved {Count} labels for board {BoardId}", labels.Count, boardId);
            return _mapper.Map<List<LabelResponse>>(labels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving labels for board {BoardId}", boardId);
            throw;
        }
    }

    public async Task<LabelResponse> AddLabel(int boardId, AddLabelDto dto)
    {
        try
        {
            Label label = new Label(dto.Title, dto.Color, boardId);
            await _labelRepository.CreateAsync(label);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Label added to board {BoardId}", boardId);
            return _mapper.Map<LabelResponse>(label);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding label to board {BoardId}", boardId);
            throw;
        }
    }

    public async Task<LabelResponse?> UpdateLabel(int labelId, UpdateLabelDto dto)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating label {LabelId}", labelId);
            throw;
        }
    }

    public async Task<bool> DeleteLabel(int labelId)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting label {LabelId}", labelId);
            throw;
        }
    }
}