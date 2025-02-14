using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.app;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence;
using Task = System.Threading.Tasks.Task;

namespace TrelloApi.Tests.Repositories;

public class UserCardRepositoryTests
{
    private readonly UserCardRepository _repository;
    private readonly TrelloContext _context;
    private readonly Mock<ILogger<UserCardRepository>> _mockLogger;

    public UserCardRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TrelloContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TrelloContext(options);
        _mockLogger = new Mock<ILogger<UserCardRepository>>();
        _repository = new UserCardRepository(_context, _mockLogger.Object);
    }
    
    [Fact]
    public async Task GetUserTaskById_ReturnsUserTask_WhenUserTaskExists()
    {
        int userId = 1, taskId = 1;
        var userTask = new UserCard(userId: 1, taskId: 1);
        
        _context.UserTasks.Add(userTask);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetUserTaskById(userId, taskId);
        
        Assert.NotNull(result);
        Assert.Equal(userTask.UserId, result.UserId);
        Assert.Equal(userTask.TaskId, result.TaskId);
    }

    [Fact]
    public async Task GetUserTaskById_ReturnsNull_WhenUserTaskDoesNotExist()
    {
        int userId = 1, taskId = 1;
        
        var result = await _repository.GetUserTaskById(userId, taskId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task AddUserTask_ReturnsUserTask_WhenUserTaskIsAddedSuccessfully()
    {
        var userTask = new UserCard(userId: 1, taskId: 1);

        _context.UserTasks.RemoveRange(_context.UserTasks);
        await _context.SaveChangesAsync();
        
        var result = await _repository.AddUserTask(userTask);
        
        Assert.NotNull(result);
        Assert.Equal(userTask.UserId, result.UserId);
        Assert.Equal(userTask.TaskId, result.TaskId);
    }

    [Fact]
    public async Task DeleteUserTask_ReturnsUserTask_WhenUserTaskIsDeletedSuccessfully()
    {
        var userTask = new UserCard(userId: 1, taskId: 1);

        _context.UserTasks.Add(userTask);
        await _context.SaveChangesAsync();
        
        var result = await _repository.DeleteUserTask(userTask);
        
        Assert.NotNull(result);
        Assert.Equal(userTask.UserId, result.UserId);
        Assert.Equal(userTask.TaskId, result.TaskId);
    }
}
