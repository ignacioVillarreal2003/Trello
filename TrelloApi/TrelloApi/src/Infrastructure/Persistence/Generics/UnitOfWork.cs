using TrelloApi.Infrastructure.Persistence.Data;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Infrastructure.Persistence.Generics;

public class UnitOfWork: IUnitOfWork
{
    public TrelloContext Context { get; }

    public UnitOfWork(TrelloContext context)
    {
        Context = context;
    }
    
    public async Task CommitAsync()
    {
        await Context.SaveChangesAsync();
    }
    
    public void Dispose()
    {
        Context.Dispose();
    }
}