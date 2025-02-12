using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.app;
using TrelloApi.Domain.Entities.TaskLabel;
using TrelloApi.Infrastructure.Persistence;

namespace TrelloApi.Tests.Repositories;

public class TaskLabelRepositoryTests
{
    private readonly TaskLabelRepository _repository;
    private readonly TrelloContext _context;
    private readonly Mock<ILogger<TaskLabelRepository>> _mockLogger;

    public TaskLabelRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TrelloContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TrelloContext(options);
        _mockLogger = new Mock<ILogger<TaskLabelRepository>>();
        _repository = new TaskLabelRepository(_context, _mockLogger.Object);
    }
    
    [Fact]
    public async Task GetTaskLabelById_ReturnsTaskLabel_WhenTaskLabelExists()
    {
        int taskId = 1, labelId = 1;
        var taskLabel = new TaskLabel(taskId: 1, labelId: 1);
        
        _context.TaskLabels.Add(taskLabel);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetTaskLabelById(taskId, labelId);
        
        Assert.NotNull(result);
        Assert.Equal(taskLabel.TaskId, result.TaskId);
        Assert.Equal(taskLabel.LabelId, result.LabelId);
    }

    [Fact]
    public async Task GetTaskLabelById_ReturnsNull_WhenTaskLabelDoesNotExist()
    {
        int taskId = 1, labelId = 1;
        
        var result = await _repository.GetTaskLabelById(taskId, labelId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task AddTaskLabel_ReturnsTaskLabel_WhenTaskLabelIsAddedSuccessfully()
    {
        var taskLabel = new TaskLabel(taskId: 1, labelId: 1);

        _context.TaskLabels.RemoveRange(_context.TaskLabels);
        await _context.SaveChangesAsync();
        
        var result = await _repository.AddTaskLabel(taskLabel);
        
        Assert.NotNull(result);
        Assert.Equal(taskLabel.TaskId, result.TaskId);
        Assert.Equal(taskLabel.LabelId, result.LabelId);
    }

    [Fact]
    public async Task DeleteTaskLabel_ReturnsTaskLabel_WhenTaskLabelIsDeletedSuccessfully()
    {
        var taskLabel = new TaskLabel(taskId: 1, labelId: 1);

        _context.TaskLabels.Add(taskLabel);
        await _context.SaveChangesAsync();
        
        var result = await _repository.DeleteTaskLabel(taskLabel);
        
        Assert.NotNull(result);
        Assert.Equal(taskLabel.TaskId, result.TaskId);
        Assert.Equal(taskLabel.LabelId, result.LabelId);
    }
}