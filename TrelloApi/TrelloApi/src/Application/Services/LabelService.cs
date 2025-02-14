using AutoMapper;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Application.Services;

public class LabelService: BaseService, ILabelService
{
    private readonly ILogger<LabelService> _logger;
    private readonly ILabelRepository _labelRepository;

    public LabelService(IMapper mapper, IBoardAuthorizationService boardAuthorizationService, ILogger<LabelService> logger, ILabelRepository labelRepository) : base(mapper, boardAuthorizationService)
    {
        _logger = logger;
        _labelRepository = labelRepository;
    }
    
    public async Task<OutputLabelDetailsDto?> GetLabelById(int labelId, int uid)
    {
        try
        {
            Label? label = await _labelRepository.GetLabelById(labelId);
            if (label == null)
            {
                _logger.LogWarning("Label {LabelId} not found", labelId);
                return null;
            }

            _logger.LogDebug("Label {LabelId} retrieved", labelId);
            return _mapper.Map<OutputLabelDetailsDto>(label);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving label {LabelId}", labelId);
            throw;
        }
    }
    
    public async Task<List<OutputLabelDetailsDto>> GetLabelsByBoardId(int boardId, int uid)
    {
        try
        {
            List<Label> labels = await _labelRepository.GetLabelsByBoardId(boardId);
            _logger.LogDebug("Retrieved {Count} labels for board {BoardId}", labels.Count, boardId);
            return _mapper.Map<List<OutputLabelDetailsDto>>(labels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving labels for board {BoardId}", boardId);
            throw;
        }
    }

    public async Task<OutputLabelDetailsDto?> AddLabel(int boardId, AddLabelDto dto, int uid)
    {
        try
        {
            Label label = new Label(dto.Title, dto.Color, boardId);
            Label? newLabel = await _labelRepository.AddLabel(label);
            if (newLabel == null)
            {
                _logger.LogError("Failed to add label to board {BoardId}", boardId);
                return null;
            }

            _logger.LogInformation("Label added to board {BoardId}", boardId);
            return _mapper.Map<OutputLabelDetailsDto>(newLabel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding label to board {BoardId}", boardId);
            throw;
        }
    }

    public async Task<OutputLabelDetailsDto?> UpdateLabel(int labelId, UpdateLabelDto dto, int uid)
    {
        try
        {
            Label? label = await _labelRepository.GetLabelById(labelId);
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

            Label? updatedLabel = await _labelRepository.UpdateLabel(label);
            if (updatedLabel == null)
            {
                _logger.LogError("Failed to update label {LabelId}", labelId);
                return null;
            }
            
            _logger.LogInformation("Label {LabelId} updated", labelId);
            return _mapper.Map<OutputLabelDetailsDto>(updatedLabel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating label {LabelId}", labelId);
            throw;
        }
    }

    public async Task<bool> DeleteLabel(int labelId, int uid)
    {
        try
        {
            Label? label = await _labelRepository.GetLabelById(labelId);
            if (label == null)
            {
                _logger.LogWarning("Label {LabelId} not found for deletion", labelId);
                return false;
            }

            bool success = await _labelRepository.DeleteLabel(label);
            if (!success)
            {
                _logger.LogError("Failed to delete label {LabelId}", labelId);
                return false;
            }

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