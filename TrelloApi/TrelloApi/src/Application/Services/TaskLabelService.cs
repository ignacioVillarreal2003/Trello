using AutoMapper;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.Entities.TaskLabel;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;
using TrelloApi.Domain.TaskLabel.Dto;

namespace TrelloApi.Application.Services;

public class TaskLabelService: BaseService, ITaskLabelService
{
    private readonly ILogger<TaskLabelService> _logger;
    private readonly ITaskLabelRepository _taskLabelRepository;
    
    public TaskLabelService(IMapper mapper, IBoardAuthorizationService boardAuthorizationService, ILogger<TaskLabelService> logger, ITaskLabelRepository taskLabelRepository) : base(mapper, boardAuthorizationService)
    {
        _logger = logger;
        _taskLabelRepository = taskLabelRepository;
    }

    public async Task<OutputTaskLabelDto?> GetTaskLabelById(int taskId, int labelId, int userId)
    {
        try
        {
            TaskLabel? taskLabel = await _taskLabelRepository.GetTaskLabelById(taskId, labelId);
            if (taskLabel == null)
            {
                _logger.LogWarning("Label {LabelId} for task {TaskId} not found.", labelId, taskId);
                return null;
            }

            _logger.LogDebug("Label {LabelId} for task {TaskId} retrieved", labelId, taskId);
            return _mapper.Map<OutputTaskLabelDto>(taskLabel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving label {LabelId} for task {TaskId}", labelId, taskId);
            throw;
        }
    }

    public async Task<OutputTaskLabelDto?> AddTaskLabel(int taskId, int labelId, int userId)
    {
        try
        {
            TaskLabel taskLabel = new TaskLabel(taskId, labelId);
            TaskLabel? newTaskLabel = await _taskLabelRepository.AddTaskLabel(taskLabel);
            if (newTaskLabel == null)
            {
                _logger.LogError("Failed to add label {LabelId} to task {TaskId}", labelId, taskId);
                return null;
            }
            
            _logger.LogInformation("Label {LabelId} added to task {TaskId}", labelId, taskId);
            return _mapper.Map<OutputTaskLabelDto>(newTaskLabel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding label {LabelId} for task {TaskId}", labelId, taskId);
            throw;
        }
    }

    public async Task<OutputTaskLabelDto?> DeleteTaskLabel(int taskId, int labelId, int userId)
    {
        try
        {
            TaskLabel? taskLabel = await _taskLabelRepository.GetTaskLabelById(taskId, labelId);
            if (taskLabel == null)
            {
                _logger.LogWarning("Label {LabelId} for task {TaskId} not found for deletion", labelId, taskId);
                return null;
            }

            TaskLabel? deletedTaskLabel = await _taskLabelRepository.DeleteTaskLabel(taskLabel);
            if (deletedTaskLabel == null)
            {
                _logger.LogError("Failed to delete label {LabelId} for task {TaskId}", labelId, taskId);
                return null;
            }

            _logger.LogInformation("Label {LabelId} for task {TaskId} deleted", labelId, taskId);
            return _mapper.Map<OutputTaskLabelDto>(deletedTaskLabel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting label {LabelId} for task {TaskId}", labelId, taskId);
            throw;
        }
    }
}