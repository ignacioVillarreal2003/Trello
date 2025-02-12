using TrelloApi.Domain.Entities.UserTask;
using TrelloApi.Domain.UserTask.Dto;

namespace TrelloApi.Domain.Interfaces.Services;

public interface IUserTaskService
{
    Task<OutputUserTaskDto?> GetUserTaskById(int userId, int taskId);
    Task<OutputUserTaskDto?> AddUserTask(AddUserTaskDto addUserTaskDto, int userId);
    Task<OutputUserTaskDto?> DeleteUserTask(int userToDeleteId, int taskId, int userId);
}