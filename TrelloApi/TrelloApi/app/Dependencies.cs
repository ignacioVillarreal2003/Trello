using TrelloApi.Application.Services;
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
        services.AddScoped<ICardService, CardService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserBoardService, UserBoardService>();
        services.AddScoped<ICardLabelService, CardLabelService>();
        services.AddScoped<IUserCardService, UserCardService>();
        services.AddScoped<IBoardAuthorizationService, BoardAuthorizationService>();
        services.AddScoped<IJwt, Jwt>();
        services.AddScoped<IEncrypt, Encrypt>();
        return services;
    }
}