using AutoMapper;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.Entities.Label;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;
using TrelloApi.Domain.Label;
using TrelloApi.Domain.Label.DTO;

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
    
    public async Task<OutputLabelDto?> GetLabelById(int labelId, int userId)
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
            return _mapper.Map<OutputLabelDto>(label);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving label {LabelId}", labelId);
            throw;
        }
    }

    public async Task<List<OutputLabelDto>> GetLabelsByTaskId(int taskId, int userId)
    {
        try
        {
            List<Label> labels = await _labelRepository.GetLabelsByTaskId(taskId);
            _logger.LogDebug("Retrieved {Count} labels for task {TaskId}", labels.Count, taskId);
            return _mapper.Map<List<OutputLabelDto>>(labels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving labels for task {TaskId}", taskId);
            throw;
        }
    }

    public async Task<OutputLabelDto?> AddLabel(int boardId, AddLabelDto addLabelDto, int userId)
    {
        try
        {
            Label label = new Label(addLabelDto.Title, addLabelDto.Color, boardId);
            Label? newLabel = await _labelRepository.AddLabel(label);
            if (newLabel == null)
            {
                _logger.LogError("Failed to add label to board {BoardId}", boardId);
                return null;
            }

            _logger.LogInformation("Label added to board {BoardId}", boardId);
            return _mapper.Map<OutputLabelDto>(newLabel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding label to board {BoardId}", boardId);
            throw;
        }
    }

    public async Task<OutputLabelDto?> UpdateLabel(int labelId, UpdateLabelDto updateLabelDto, int userId)
    {
        try
        {
            Label? label = await _labelRepository.GetLabelById(labelId);
            if (label == null)
            {
                _logger.LogWarning("Label {LabelId} not found for update", labelId);
                return null;
            }

            if (!string.IsNullOrEmpty(updateLabelDto.Color))
            {
                label.Color = updateLabelDto.Color;
            }
            if (!string.IsNullOrEmpty(updateLabelDto.Title))
            {
                label.Title = updateLabelDto.Title;
            }

            Label? updatedLabel = await _labelRepository.UpdateLabel(label);
            if (updatedLabel == null)
            {
                _logger.LogError("Failed to update label {LabelId}", labelId);
                return null;
            }
            
            _logger.LogInformation("Label {LabelId} updated", labelId);
            return _mapper.Map<OutputLabelDto>(updatedLabel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating label {LabelId}", labelId);
            throw;
        }
    }

    public async Task<OutputLabelDto?> DeleteLabel(int labelId, int userId)
    {
        try
        {
            Label? label = await _labelRepository.GetLabelById(labelId);
            if (label == null)
            {
                _logger.LogWarning("Label {LabelId} not found for deletion", labelId);
                return null;
            }

            Label? deletedLabel = await _labelRepository.DeleteLabel(label);
            if (deletedLabel == null)
            {
                _logger.LogError("Failed to delete label {LabelId}", labelId);
                return null;
            }

            _logger.LogInformation("Label {LabelId} deleted", labelId);
            return _mapper.Map<OutputLabelDto>(deletedLabel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting label {LabelId}", labelId);
            throw;
        }
    }
}