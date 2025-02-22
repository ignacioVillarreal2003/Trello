using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.app;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence;
using TrelloApi.Infrastructure.Persistence.Data;
using TrelloApi.Infrastructure.Persistence.Repositories;
using Task = System.Threading.Tasks.Task;

namespace TrelloApi.Tests.Repositories;

public class ListRepositoryTests
{
    private readonly ListRepository _repository;
    private readonly TrelloContext _context;
    private readonly Mock<ILogger<ListRepository>> _mockLogger;

    public ListRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TrelloContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TrelloContext(options);
        _mockLogger = new Mock<ILogger<ListRepository>>();
        _repository = new ListRepository(_context, _mockLogger.Object);
    }
    
    [Fact]
    public async Task GetListById_ShouldReturnList_WhenListExists()
    {
        int listId = 1;
        var list = new List(title: "title", boardId: 1, position: 0) { Id = listId };
        
        _context.Lists.Add(list);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetListById(listId);
        
        Assert.NotNull(result);
        Assert.Equal(listId, result.Id);
    }

    [Fact]
    public async Task GetListById_ShouldReturnNull_WhenListDoesNotExist()
    {
        int listId = 1;
        
        var result = await _repository.GetListById(listId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetListsByBoardId_ShouldReturnLists_WhenBoardHasLists()
    {
        int boardId = 1;
        var list1 = new List(title: "Test List", boardId: 1, position: 0) { Id = 1 };
        var list2 = new List(title: "Test List", boardId: 1, position:1) { Id = 2 };

        _context.Lists.AddRange(list1, list2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetListsByBoardId(boardId);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetListsByBoardId_ShouldReturnEmptyList_WhenBoardHasNoLists()
    {
        int boardId = 1;
        
        var result = await _repository.GetListsByBoardId(boardId);
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddList_ShouldPersistList_WhenAddedSuccessfully()
    {
        var list = new List(title: "Test List", boardId: 1, position: 0) { Id = 1 };
        
        _context.Lists.RemoveRange(_context.Lists);
        await _context.SaveChangesAsync();
        
        await _repository.AddList(list);
        var result = await _context.Lists.FindAsync(list.Id);
        
        Assert.NotNull(result);
        Assert.Equal(list.Id, result.Id);
    }

    [Fact]
    public async Task UpdateList_ShouldPersistChanges_WhenUpdateIsSuccessful()
    {
        var list = new List(title: "title", boardId: 1, position: 0) { Id = 1 };

        _context.Lists.Add(list);
        await _context.SaveChangesAsync();
        
        list.Title = "Updated title";
        await _repository.UpdateList(list);
        var result = await _context.Lists.FindAsync(list.Id);
        
        Assert.NotNull(result);
        Assert.Equal(list.Title, result.Title);
    }

    [Fact]
    public async Task DeleteList_ShouldRemoveList_WhenListExists()
    {
        var list = new List(title: "title", boardId: 1, position: 0) { Id = 1 };

        _context.Lists.Add(list);
        await _context.SaveChangesAsync();
        
        await _repository.DeleteList(list);
        var result = await _context.Lists.FindAsync(list.Id);
        
        Assert.Null(result);
    }
}
