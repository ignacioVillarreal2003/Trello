using TrelloApi.Application.Services;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Application.Utils;
using TrelloApi.Domain.Interfaces.Services;
using TrelloApi.Infrastructure.Authentication;

namespace TrelloApi.app;

public static class Dependencies
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IBoardService, BoardService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<ILabelService, LabelService>();
        services.AddScoped<IListService, ListService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserBoardService, UserBoardService>();
        services.AddScoped<ITaskLabelService, TaskLabelService>();
        services.AddScoped<IUserTaskService, UserTaskService>();
        services.AddScoped<IBoardAuthorizationService, BoardAuthorizationService>();
        services.AddScoped<IJwt, Jwt>();
        services.AddScoped<IEncrypt, Encrypt>();
        return services;
    }
}