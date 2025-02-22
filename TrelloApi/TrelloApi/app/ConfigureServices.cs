using TrelloApi.Application.Services;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Application.Utils;

namespace TrelloApi.app;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IBoardService, BoardService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<ILabelService, LabelService>();
        services.AddScoped<IListService, ListService>();
        services.AddScoped<ICardService, CardService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserBoardService, UserBoardService>();
        services.AddScoped<ICardLabelService, CardLabelService>();
        services.AddScoped<IUserCardService, UserCardService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IEncrypt, Encrypt>();
        return services;
    }
}