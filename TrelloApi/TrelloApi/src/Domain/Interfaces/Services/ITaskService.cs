using TrelloApi.Domain.Entities.Task;
using TrelloApi.Domain.Task.DTO;

namespace TrelloApi.Domain.Interfaces.Services;

public interface ITaskService
{
    Task<OutputTaskDto?> GetTaskById(int taskId, int userId);
    Task<List<OutputTaskDto>> GetTasksByListId(int listId, int userId);
    Task<OutputTaskDto?> AddTask(int listId, AddTaskDto addTaskDto, int userId);
    Task<OutputTaskDto?> UpdateTask(int taskId, UpdateTaskDto updateTaskDto, int userId);
    Task<OutputTaskDto?> DeleteTask(int taskId, int userId);
}