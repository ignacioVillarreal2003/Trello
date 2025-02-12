using AutoMapper;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.Entities.Task;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;
using TrelloApi.Domain.Task;
using TrelloApi.Domain.Task.DTO;
using TrelloApi.Infrastructure.Persistence;
using Task = TrelloApi.Domain.Task.Task;

namespace TrelloApi.Application.Services;

public class TaskService: BaseService, ITaskService
{
    private readonly ILogger<TaskService> _logger;
    private readonly ITaskRepository _taskRepository;

    public TaskService(IMapper mapper, IBoardAuthorizationService boardAuthorizationService, ILogger<TaskService> logger, ITaskRepository taskRepository): base(mapper, boardAuthorizationService)
    {
        _logger = logger;
        _taskRepository = taskRepository;
    }
    
    public async Task<OutputTaskDto?> GetTaskById(int taskId, int userId)
    {
        try
        {
            Task? task = await _taskRepository.GetTaskById(taskId);
            if (task == null)
            {
                _logger.LogWarning("Task {TaskId} not found", taskId);
                return null;
            }

            _logger.LogDebug("Task {TaskId} retrieved", taskId);
            return _mapper.Map<OutputTaskDto>(task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving task {TaskId}", taskId);
            throw;
        }
    }

    public async Task<List<OutputTaskDto>> GetTasksByListId(int listId, int userId)
    {
        try
        {
            List<Task> tasks = await _taskRepository.GetTasksByListId(listId);
            _logger.LogDebug("Retrieved {Count} tasks for list {ListId}", tasks.Count, listId);
            return _mapper.Map<List<OutputTaskDto>>(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks for list {ListId}", listId);
            throw;
        }
    }

    public async Task<OutputTaskDto?> AddTask(int listId, AddTaskDto addTaskDto, int userId)
    {
        try
        {
            Task task = new Task(addTaskDto.Title, addTaskDto.Description, listId);
            Task? newTask = await _taskRepository.AddTask(task);
            if (newTask == null)
            {
                _logger.LogError("Failed to add task to list {ListId}", listId);
                return null;
            }

            _logger.LogInformation("Task added to list {ListId}", listId);
            return _mapper.Map<OutputTaskDto>(newTask);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding task to list {ListId}", listId);
            throw;
        }
    }

    public async Task<OutputTaskDto?> UpdateTask(int taskId, UpdateTaskDto updateTaskDto, int userId)
    {
        try
        {
            Task? task = await _taskRepository.GetTaskById(taskId);
            if (task == null)
            {
                _logger.LogWarning("Task {TaskId} not found for update", taskId);
                return null;
            }

            if (!string.IsNullOrEmpty(updateTaskDto.Title))
            {
                task.Title = updateTaskDto.Title;
            }
            if (!string.IsNullOrEmpty(updateTaskDto.Description))
            {
                task.Description = updateTaskDto.Description;
            }
            if (updateTaskDto.ListId.HasValue)
            {
                task.ListId = updateTaskDto.ListId.Value;
            }

            Task? updatedTask = await _taskRepository.UpdateTask(task);
            if (updatedTask == null)
            {
                _logger.LogError("Failed to update task {TaskId}", taskId);
                return null;
            }
            
            _logger.LogInformation("Task {TaskId} updated", taskId);
            return _mapper.Map<OutputTaskDto>(updatedTask);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task {TaskId}", taskId);
            throw;
        }
    }

    public async Task<OutputTaskDto?> DeleteTask(int taskId, int userId)
    {
        try
        {
            Task? task = await _taskRepository.GetTaskById(taskId);
            if (task == null)
            {
                _logger.LogWarning("Task {TaskId} not found for update", taskId);
                return null;
            }

            Task? deletedTask = await _taskRepository.DeleteTask(task);
            if (deletedTask == null)
            {
                _logger.LogError("Failed to delete task {TaskId}", taskId);
                return null;
            }

            _logger.LogInformation("Task {TaskId} deleted", taskId);
            return _mapper.Map<OutputTaskDto>(deletedTask);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task {TaskId}", taskId);
            throw;
        }
    }
}