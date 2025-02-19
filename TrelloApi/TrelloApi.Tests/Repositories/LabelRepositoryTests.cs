using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.app;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence;

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
    public async Task GetLabelById_ShouldReturnLabel_WhenLabelExists()
    {
        int labelId = 1;
        var label = new Label(title: "title", color: "color", boardId: 1) { Id = labelId };
        
        _context.Labels.Add(label);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetLabelById(labelId);
        
        Assert.NotNull(result);
        Assert.Equal(labelId, result.Id);
    }

    [Fact]
    public async Task GetLabelById_ShouldReturnNull_WhenLabelDoesNotExist()
    {
        int labelId = 1;
        
        var result = await _repository.GetLabelById(labelId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetLabelsByBoardId_ShouldReturnLabels_WhenBoardHasLabels()
    {
        int boardId = 1;
        var label1 = new Label(title: "title 1", color: "color", boardId: boardId) { Id = 1 };
        var label2 = new Label(title: "title 1", color: "color", boardId: boardId) { Id = 2 };
        
        _context.Labels.AddRange(label1, label2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetLabelsByBoardId(boardId);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetLabelsByBoardId_ShouldReturnEmptyList_WhenBoardHasNoLabels()
    {
        int boardId = 1;
        
        var result = await _repository.GetLabelsByBoardId(boardId);
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddLabel_ShouldPersistLabel_WhenAddedSuccessfully()
    {
        var label = new Label(title: "title", color: "color", boardId: 1) { Id = 1 };
        
        await _repository.AddLabel(label);
        var result = await _context.Labels.FindAsync(label.Id);

        Assert.NotNull(result);
        Assert.Equal(label.Id, result.Id);
    }

    [Fact]
    public async Task UpdateLabel_ReturnsLabel_WhenLabelIsUpdatedSuccessfully()
    {
        var label = new Label(title: "title", color: "color", boardId: 1) { Id = 1 };

        _context.Labels.Add(label);
        await _context.SaveChangesAsync();
        
        label.Title = "updated title";
        await _repository.UpdateLabel(label);
        var result = await _context.Labels.FindAsync(label.Id);
        
        Assert.NotNull(result);
        Assert.Equal(label.Title, result.Title);
    }

    [Fact]
    public async Task DeleteLabel_ReturnsLabel_WhenLabelIsDeletedSuccessfully()
    {
        var label = new Label(title: "title", color: "color", boardId: 1) { Id = 1 };

        _context.Labels.Add(label);
        await _context.SaveChangesAsync();
        
        await _repository.DeleteLabel(label);
        var result = await _context.Labels.FindAsync(label.Id);

        Assert.Null(result);
    }
}
