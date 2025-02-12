using TrelloApi.Domain.Board;
using TrelloApi.Domain.Comment;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Label;
using TrelloApi.Domain.Task;
using TrelloApi.Domain.User;
using TrelloApi.Domain.UserBoard;
using TrelloApi.Domain.UserTask;
using TrelloApi.Infrastructure.Persistence;

namespace TrelloApi.app;

public static class Repositories
{
    public static IServiceCollection AddApplicationRepositories(this IServiceCollection repositories)
    {
        repositories.AddScoped<IBoardRepository, BoardRepository>();
        repositories.AddScoped<ICommentRepository, CommentRepository>();
        repositories.AddScoped<ILabelRepository, LabelRepository>();
        repositories.AddScoped<IListRepository, ListRepository>();
        repositories.AddScoped<ITaskRepository, TaskRepository>();
        repositories.AddScoped<IUserRepository, UserRepository>();
        repositories.AddScoped<IUserBoardRepository, UserBoardRepository>();
        repositories.AddScoped<ITaskLabelRepository, TaskLabelRepository>();
        repositories.AddScoped<IUserTaskRepository, UserTaskRepository>();
        return repositories;
    }
}