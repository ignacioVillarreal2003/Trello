using Microsoft.EntityFrameworkCore;
using TrelloApi.app;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;

namespace TrelloApi.Infrastructure.Persistence;

public class ListRepository: Repository<List>, IListRepository
{
    private readonly ILogger<ListRepository> _logger;

    public ListRepository(TrelloContext context, ILogger<ListRepository> logger): base(context)
    {
        _logger = logger;
    }
    
    public async Task<List?> GetListById(int listId)
    {
        try
        {
            List? list = await Context.Lists.FirstOrDefaultAsync(l => l.Id == listId);
            
            _logger.LogDebug("List {ListId} retrieval attempt completed", listId);
            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving list {ListId}", listId);
            throw;
        }
    }

    public async Task<List<List>> GetListsByBoardId(int boardId)
    {
        try
        {
            List<List> lists = await Context.Lists
                .Where(l => l.BoardId == boardId)
                .ToListAsync();
            
            _logger.LogDebug("Retrieved {Count} lists for board {BoardId}", lists.Count, boardId);
            return lists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving lists for board {BoardId}", boardId);
            throw;
        }
    }

    public async Task<List?> AddList(List list)
    {
        try
        {
            await Context.Lists.AddAsync(list);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("List {ListId} added to board {BoardId}", list.Id, list.BoardId);
            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error adding list to board {BoardId}", list.BoardId);
            throw;
        }
    }

    public async Task<List?> UpdateList(List list)
    {
        try
        {
            Context.Lists.Update(list);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("List {ListId} updated", list.Id);
            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error updating list {ListId}", list.Id);
            throw;
        }
    }

    public async Task<List?> DeleteList(List list)
    {
        try
        {
            Context.Lists.Remove(list);
            await Context.SaveChangesAsync();
            
            _logger.LogDebug("List {ListId} deleted", list.Id);
            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error deleting list {ListId}", list.Id);
            throw;
        }
    }
}