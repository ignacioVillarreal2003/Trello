using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.app;
using TrelloApi.Infrastructure.Persistence;

namespace TrelloApi.Tests.Repositories;

public class CardRepositoryTests
{
    private readonly CardRepository _repository;
    private readonly TrelloContext _context;
    private readonly Mock<ILogger<CardRepository>> _mockLogger;

    public CardRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TrelloContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TrelloContext(options);
        _mockLogger = new Mock<ILogger<CardRepository>>();
        _repository = new CardRepository(_context, _mockLogger.Object);
    }
    
    [Fact]
    public async Task GetTaskById_ReturnsTask_WhenTaskExists()
    {
        int taskId = 1;
        var task = new Domain.Entities.Card.Task(title: "Test Task", description: "", listId: 1) { Id = 1 };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetTaskById(taskId);
        
        Assert.NotNull(result);
        Assert.Equal(taskId, result.Id);
    }

    [Fact]
    public async Task GetTaskById_ReturnsNull_WhenTaskDoesNotExist()
    {
        int taskId = 1;
        
        var result = await _repository.GetTaskById(taskId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetTasks_ReturnsTasks_WhenTasksExistForUser()
    {
        int listId = 1;
        var task1 = new Domain.Entities.Card.Task(title: "Task 1", description: "", listId: 1) { Id = 1 };
        var task2 = new Domain.Entities.Card.Task(title: "Task 2", description: "", listId: 1) { Id = 2 };

        _context.Tasks.AddRange(task1, task2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetTasksByListId(listId);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetTasks_ReturnsEmptyList_WhenNoTasksExistForUser()
    {
        int listId = 1;
        
        var result = await _repository.GetTasksByListId(listId);
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddTask_ReturnsTask_WhenTaskIsAddedSuccessfully()
    {
        var task = new Domain.Entities.Card.Task(title: "New task", description: "", listId: 1) { Id = 1 };

        _context.Tasks.RemoveRange(_context.Tasks);
        await _context.SaveChangesAsync();
        
        var result = await _repository.AddTask(task);
        
        Assert.NotNull(result);
        Assert.Equal(task.Id, result.Id);
    }

    [Fact]
    public async Task UpdateTask_ReturnsTask_WhenTaskIsUpdatedSuccessfully()
    {
        var task = new Domain.Entities.Card.Task(title: "Existing task", description: "", listId: 1) { Id = 1 };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        task.Title = "Updated task";
        
        var result = await _repository.UpdateTask(task);
        
        Assert.NotNull(result);
        Assert.Equal(task.Id, result.Id);
    }

    [Fact]
    public async Task DeleteTask_ReturnsTask_WhenTaskIsDeletedSuccessfully()
    {
        var task = new Domain.Entities.Card.Task(title: "Task To Delete", description: "", listId: 1) { Id = 1 };
        
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        
        var result = await _repository.DeleteTask(task);
        
        Assert.NotNull(result);
        Assert.Equal(task.Id, result.Id);
    }
}
