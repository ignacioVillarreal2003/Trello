using TrelloApi.Domain.Interfaces.Repositories;
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
        repositories.AddScoped<ICardRepository, CardRepository>();
        repositories.AddScoped<IUserRepository, UserRepository>();
        repositories.AddScoped<IUserBoardRepository, UserBoardRepository>();
        repositories.AddScoped<ICardLabelRepository, CardLabelRepository>();
        repositories.AddScoped<IUserCardRepository, UserCardRepository>();
        return repositories;
    }
}