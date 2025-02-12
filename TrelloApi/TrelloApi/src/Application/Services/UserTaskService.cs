using AutoMapper;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.Entities.UserTask;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Application.Services;

public class UserTaskService: BaseService, IUserTaskService
{
    private readonly ILogger<UserTaskService> _logger;
    private readonly IUserTaskRepository _userTaskRepository;
    
    public UserTaskService(IMapper mapper, IBoardAuthorizationService boardAuthorizationService, ILogger<UserTaskService> logger, IUserTaskRepository userTaskRepository) : base(mapper, boardAuthorizationService)
    {
        _logger = logger;
        _userTaskRepository = userTaskRepository;
    }

    public async Task<OutputUserTaskDto?> GetUserTaskById(int userId, int taskId)
    {
        try
        {
            Domain.Entities.UserTask.UserTask? userTask = await _userTaskRepository.GetUserTaskById(userId, taskId);
            if (userTask == null)
            {
                _logger.LogWarning("Task {TaskId} for user {UserId} not found.", taskId, userId);
                return null;
            }

            _logger.LogDebug("Task {TaskId} for user {UserId} retrieved", taskId, userId);
            return _mapper.Map<OutputUserTaskDto>(userTask);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving task {TaskId} for user {UserId}", taskId, userId);
            throw;
        }
    }

    public async Task<OutputUserTaskDto?> AddUserTask(AddUserTaskDto addUserTaskDto, int userId)
    {
        try
        {
            Domain.Entities.UserTask.UserTask userTask = new Domain.Entities.UserTask.UserTask(addUserTaskDto.UserId, addUserTaskDto.TaskId);
            Domain.Entities.UserTask.UserTask? newUserTask = await _userTaskRepository.AddUserTask(userTask);
            if (newUserTask == null)
            {
                _logger.LogError("Failed to add task {TaskId} to user {UserId}", addUserTaskDto.TaskId, addUserTaskDto.UserId);
                return null;
            }
            
            _logger.LogInformation("Task {TaskId} added to User {UserId}", addUserTaskDto.TaskId, addUserTaskDto.UserId);
            return _mapper.Map<OutputUserTaskDto>(newUserTask);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding task {TaskId} to user {UserId}", addUserTaskDto.TaskId, addUserTaskDto.UserId);
            throw;
        }
    }

    public async Task<OutputUserTaskDto?> DeleteUserTask(int userToDeleteId, int taskId, int userId)
    {
        try
        {
            Domain.Entities.UserTask.UserTask? userTask = await _userTaskRepository.GetUserTaskById(userToDeleteId, taskId);
            if (userTask == null)
            {
                _logger.LogWarning("Task {TaskId} to user {UserId} not found for deletion", taskId, userToDeleteId);
                return null;
            }

            Domain.Entities.UserTask.UserTask? deletedUserTask = await _userTaskRepository.DeleteUserTask(userTask);
            if (deletedUserTask == null)
            {
                _logger.LogError("Failed to delete task {TaskId} to user {UserId}", taskId, userToDeleteId);
                return null;
            }

            _logger.LogInformation("Task {TaskId} to user {UserId} deleted", taskId, userToDeleteId);
            return _mapper.Map<OutputUserTaskDto>(deletedUserTask);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task {TaskId} to user {UserId}", taskId, userToDeleteId);
            throw;
        }
    }
}