using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.app;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Label;
using TrelloApi.Infrastructure.Persistence;
using Task = System.Threading.Tasks.Task;

namespace TrelloApi.Tests.Repositories;

public class LabelRepositoryTests
{
    private readonly LabelRepository _repository;
    private readonly TrelloContext _context;
    private readonly Mock<ILogger<LabelRepository>> _mockLogger;

    public LabelRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TrelloContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TrelloContext(options);
        _mockLogger = new Mock<ILogger<LabelRepository>>();
        _repository = new LabelRepository(_context, _mockLogger.Object);
    }
    
    [Fact]
    public async Task GetLabelById_ReturnsLabel_WhenLabelExists()
    {
        int labelId = 1;
        var label = new Label(title: "Test Label", color: "Blue", boardId: 1) { Id = labelId };
        
        _context.Labels.Add(label);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetLabelById(labelId);
        
        Assert.NotNull(result);
        Assert.Equal(labelId, result.Id);
    }

    [Fact]
    public async Task GetLabelById_ReturnsNull_WhenLabelDoesNotExist()
    {
        int labelId = 1;
        
        var result = await _repository.GetLabelById(labelId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetLabelsByTaskId_ReturnsLabels_WhenLabelsExistForUser()
    {
        int taskId = 1;
        var label1 = new Label(title: "Test Label", color: "Blue", boardId: 1) { Id = 1 };
        var label2 = new Label(title: "Test Label", color: "Blue", boardId: 1) { Id = 2 };
        var taskLabel1 = new CardLabel(1, 1);
        var taskLabel2 = new CardLabel(1, 2);
        
        _context.Labels.AddRange(label1, label2);
        _context.TaskLabels.AddRange(taskLabel1, taskLabel2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetLabelsByTaskId(taskId);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetLabelsByTaskId_ReturnsEmptyList_WhenNoLabelsExistForUser()
    {
        int taskId = 1;
        
        var result = await _repository.GetLabelsByTaskId(taskId);
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddLabel_ReturnsLabel_WhenLabelIsAddedSuccessfully()
    {
        var label = new Label(title: "New Label", color: "Blue", boardId: 1) { Id = 1 };

        _context.Labels.RemoveRange(_context.Labels);
        await _context.SaveChangesAsync();
        
        var result = await _repository.AddLabel(label);
        
        Assert.NotNull(result);
        Assert.Equal(label.Id, result.Id);
    }

    [Fact]
    public async Task UpdateLabel_ReturnsLabel_WhenLabelIsUpdatedSuccessfully()
    {
        var label = new Label(title: "Existing Label", color: "Blue", boardId: 1) { Id = 1 };

        _context.Labels.Add(label);
        await _context.SaveChangesAsync();
        label.Title = "Updated Label";
        
        var result = await _repository.UpdateLabel(label);
        
        Assert.NotNull(result);
        Assert.Equal(label.Id, result.Id);
    }

    [Fact]
    public async Task DeleteLabel_ReturnsLabel_WhenLabelIsDeletedSuccessfully()
    {
        var label = new Label(title: "Label To Delete", color: "Blue", boardId: 1) { Id = 1 };

        _context.Labels.Add(label);
        await _context.SaveChangesAsync();
        
        var result = await _repository.DeleteLabel(label);
        
        Assert.NotNull(result);
        Assert.Equal(label.Id, result.Id);
    }
}
