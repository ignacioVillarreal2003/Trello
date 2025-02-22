using TrelloApi.Infrastructure.Persistence.Generics;
using TrelloApi.Infrastructure.Persistence.Interfaces;
using TrelloApi.Infrastructure.Persistence.Repositories;

namespace TrelloApi.app;

public static class ConfigureRepositories
{
    public static IServiceCollection AddApplicationRepositories(this IServiceCollection repositories)
    {
        repositories.AddScoped<IUnitOfWork, UnitOfWork>();
        repositories.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
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