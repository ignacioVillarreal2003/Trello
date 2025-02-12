using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.app;
using TrelloApi.Domain.Entities.UserTask;
using TrelloApi.Infrastructure.Persistence;

namespace TrelloApi.Tests.Repositories;

public class UserTaskRepositoryTests
{
    private readonly UserTaskRepository _repository;
    private readonly TrelloContext _context;
    private readonly Mock<ILogger<UserTaskRepository>> _mockLogger;

    public UserTaskRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TrelloContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TrelloContext(options);
        _mockLogger = new Mock<ILogger<UserTaskRepository>>();
        _repository = new UserTaskRepository(_context, _mockLogger.Object);
    }
    
    [Fact]
    public async Task GetUserTaskById_ReturnsUserTask_WhenUserTaskExists()
    {
        int userId = 1, taskId = 1;
        var userTask = new UserTask(userId: 1, taskId: 1);
        
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
        var userTask = new UserTask(userId: 1, taskId: 1);

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
        var userTask = new UserTask(userId: 1, taskId: 1);

        _context.UserTasks.Add(userTask);
        await _context.SaveChangesAsync();
        
        var result = await _repository.DeleteUserTask(userTask);
        
        Assert.NotNull(result);
        Assert.Equal(userTask.UserId, result.UserId);
        Assert.Equal(userTask.TaskId, result.TaskId);
    }
}
