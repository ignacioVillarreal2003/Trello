using TrelloApi.app;
using TrelloApi.Infrastructure.Persistence.Data;

namespace TrelloApi.Infrastructure.Persistence.Interfaces;

public interface IUnitOfWork: IDisposable
{
    TrelloContext Context { get; }
    Task CommitAsync();
}