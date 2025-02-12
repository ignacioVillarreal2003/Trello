using TrelloApi.Domain.Entities.TaskLabel;

namespace TrelloApi.Domain.Interfaces.Services;

public interface ITaskLabelService
{
    Task<OutputTaskLabelDto?> GetTaskLabelById(int taskId, int labelId, int userId);
    Task<OutputTaskLabelDto?> AddTaskLabel(int taskId, int labelId, int userId);
    Task<OutputTaskLabelDto?> DeleteTaskLabel(int taskId, int labelId, int userId);
}