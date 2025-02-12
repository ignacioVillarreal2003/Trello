namespace TrelloApi.Domain.Interfaces.Repositories;

public interface ITaskRepository
{
    Task<Task.Task?> GetTaskById(int taskId);
    Task<List<Task.Task>> GetTasksByListId(int listId);
    Task<Task.Task?> AddTask(Task.Task task);
    Task<Task.Task?> UpdateTask(Task.Task task);
    Task<Task.Task?> DeleteTask(Task.Task task);
}