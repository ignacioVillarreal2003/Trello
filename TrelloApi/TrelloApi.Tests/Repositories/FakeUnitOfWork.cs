using TrelloApi.Infrastructure.Persistence.Data;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Tests.Repositories;

public class FakeUnitOfWork : IUnitOfWork
{
    public TrelloContext Context { get; }
    public FakeUnitOfWork(TrelloContext context)
    {
        Context = context;
    }
    public Task CommitAsync() => Context.SaveChangesAsync();
    public void Dispose() => Context.Dispose();
}