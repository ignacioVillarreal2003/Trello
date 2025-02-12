using TrelloApi.Domain.Entities.TaskLabel;

namespace TrelloApi.Domain.Interfaces.Repositories;

public interface ITaskLabelRepository
{
    Task<TaskLabel?> GetTaskLabelById(int taskId, int labelId);
    Task<TaskLabel?> AddTaskLabel(TaskLabel taskLabel);
    Task<TaskLabel?> DeleteTaskLabel(TaskLabel taskLabel);
}