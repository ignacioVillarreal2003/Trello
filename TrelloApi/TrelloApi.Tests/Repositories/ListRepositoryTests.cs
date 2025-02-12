using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.app;
using TrelloApi.Domain.Entities.List;
using TrelloApi.Infrastructure.Persistence;

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
    public async Task GetListById_ReturnsList_WhenListExists()
    {
        int listId = 1;
        var list = new List(title: "Test List", boardId: 1) { Id = listId };
        
        _context.Lists.Add(list);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetListById(listId);
        
        Assert.NotNull(result);
        Assert.Equal(listId, result.Id);
    }

    [Fact]
    public async Task GetListById_ReturnsNull_WhenListDoesNotExist()
    {
        int listId = 1;
        
        var result = await _repository.GetListById(listId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetListsByBoardId_ReturnsLists_WhenListsExistForBoard()
    {
        int boardId = 1;
        var list1 = new List(title: "Test List", boardId: 1) { Id = 1 };
        var list2 = new List(title: "Test List", boardId: 1) { Id = 2 };

        _context.Lists.AddRange(list1, list2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetListsByBoardId(boardId);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetListsByBoardId_ReturnsEmptyList_WhenNoListsExistForBoard()
    {
        int boardId = 1;
        
        var result = await _repository.GetListsByBoardId(boardId);
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddList_ReturnsList_WhenListIsAddedSuccessfully()
    {
        var list = new List(title: "Test List", boardId: 1) { Id = 1 };
        
        _context.Lists.RemoveRange(_context.Lists);
        await _context.SaveChangesAsync();
        
        var result = await _repository.AddList(list);
        
        Assert.NotNull(result);
        Assert.Equal(list.Id, result.Id);
    }

    [Fact]
    public async Task UpdateList_ReturnsList_WhenListIsUpdatedSuccessfully()
    {
        var list = new List(title: "Existing List", boardId: 1) { Id = 1 };

        _context.Lists.Add(list);
        await _context.SaveChangesAsync();
        list.Title = "Updated List";
        
        var result = await _repository.UpdateList(list);
        
        Assert.NotNull(result);
        Assert.Equal(list.Id, result.Id);
    }

    [Fact]
    public async Task DeleteList_ReturnsList_WhenListIsDeletedSuccessfully()
    {
        var list = new List(title: "List To Delete", boardId: 1) { Id = 1 };

        _context.Lists.Add(list);
        await _context.SaveChangesAsync();
        
        var result = await _repository.DeleteList(list);
        
        Assert.NotNull(result);
        Assert.Equal(list.Id, result.Id);
    }
}
