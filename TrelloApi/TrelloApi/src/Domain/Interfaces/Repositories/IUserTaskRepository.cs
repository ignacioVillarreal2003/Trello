namespace TrelloApi.Domain.Interfaces.Repositories;

public interface IUserTaskRepository
{
    Task<Entities.UserTask.UserTask?> GetUserTaskById(int userId, int taskId);
    Task<Entities.UserTask.UserTask?> AddUserTask(Entities.UserTask.UserTask userBoard);
    Task<Entities.UserTask.UserTask?> DeleteUserTask(Entities.UserTask.UserTask userBoard);
}