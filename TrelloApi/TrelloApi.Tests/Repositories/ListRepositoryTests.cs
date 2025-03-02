using Microsoft.EntityFrameworkCore;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Data;
using TrelloApi.Infrastructure.Persistence.Interfaces;
using TrelloApi.Infrastructure.Persistence.Repositories;

namespace TrelloApi.Tests.Repositories;

public class ListRepositoryTests
{
    private readonly IListRepository _repository;
    private readonly TrelloContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public ListRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TrelloContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TrelloContext(options);
        _unitOfWork = new FakeUnitOfWork(_context);
        _repository = new ListRepository(_unitOfWork);
    }
    
    [Fact]
    public async Task GetListById_ShouldReturnList_WhenListExists()
    {
        int listId = 1;
        var list = new List("title", boardId: 1, position: 0) { Id = listId };
        
        _context.Lists.Add(list);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetAsync(l => l.Id == listId);
        
        Assert.NotNull(result);
        Assert.Equal(listId, result.Id);
    }

    [Fact]
    public async Task GetListById_ShouldReturnNull_WhenListDoesNotExist()
    {
        int listId = 1;
        
        var result = await _repository.GetAsync(l => l.Id == listId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetListsByBoardId_ShouldReturnLists_WhenBoardHasLists()
    {
        int boardId = 1;
        var list1 = new List("Test List", boardId: boardId, position: 0) { Id = 1 };
        var list2 = new List("Test List", boardId: boardId, position: 1) { Id = 2 };

        _context.Lists.AddRange(list1, list2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetListAsync(l => l.BoardId == boardId);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetListsByBoardId_ShouldReturnEmptyList_WhenBoardHasNoLists()
    {
        int boardId = 1;
        
        var result = await _repository.GetListAsync(l => l.BoardId == boardId);
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddList_ShouldPersistList_WhenAddedSuccessfully()
    {
        var list = new List("Test List", boardId: 1, position: 0) { Id = 1 };
        
        _context.Lists.RemoveRange(_context.Lists);
        await _context.SaveChangesAsync();
        
        await _repository.CreateAsync(list);
        await _unitOfWork.CommitAsync();
        
        var result = await _context.Lists.FindAsync(list.Id);
        
        Assert.NotNull(result);
        Assert.Equal(list.Id, result.Id);
    }

    [Fact]
    public async Task UpdateList_ShouldPersistChanges_WhenUpdateIsSuccessful()
    {
        var list = new List("title", boardId: 1, position: 0) { Id = 1 };

        _context.Lists.Add(list);
        await _context.SaveChangesAsync();
        
        list.Title = "Updated title";
        await _repository.UpdateAsync(list);
        await _unitOfWork.CommitAsync();
        
        var result = await _context.Lists.FindAsync(list.Id);
        
        Assert.NotNull(result);
        Assert.Equal("Updated title", result.Title);
    }

    [Fact]
    public async Task DeleteList_ShouldRemoveList_WhenListExists()
    {
        var list = new List("title", boardId: 1, position: 0) { Id = 1 };

        _context.Lists.Add(list);
        await _context.SaveChangesAsync();
        
        await _repository.DeleteAsync(list);
        await _unitOfWork.CommitAsync();
        
        var result = await _context.Lists.FindAsync(list.Id);
        
        Assert.Null(result);
    }
}