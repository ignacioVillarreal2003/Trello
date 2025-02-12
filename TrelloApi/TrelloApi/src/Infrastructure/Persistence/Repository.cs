using TrelloApi.app;

namespace TrelloApi.Infrastructure.Persistence;

public abstract class Repository<T> where T : class
{
    protected readonly TrelloContext Context;

    protected Repository(TrelloContext context)
    {
        Context = context;
    }
}