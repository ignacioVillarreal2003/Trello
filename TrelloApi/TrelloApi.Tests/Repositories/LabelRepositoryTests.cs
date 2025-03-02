using Microsoft.EntityFrameworkCore;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Data;
using TrelloApi.Infrastructure.Persistence.Interfaces;
using TrelloApi.Infrastructure.Persistence.Repositories;

namespace TrelloApi.Tests.Repositories;

public class LabelRepositoryTests
{
    private readonly ILabelRepository _repository;
    private readonly TrelloContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public LabelRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TrelloContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TrelloContext(options);
        _unitOfWork = new FakeUnitOfWork(_context);
        _repository = new LabelRepository(_unitOfWork);
    }
    
    [Fact]
    public async Task GetLabelById_ShouldReturnLabel_WhenLabelExists()
    {
        int labelId = 1;
        var label = new Label("title", "color", boardId: 1) { Id = labelId };
        
        _context.Labels.Add(label);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetAsync(l => l.Id == labelId);
        
        Assert.NotNull(result);
        Assert.Equal(labelId, result.Id);
    }

    [Fact]
    public async Task GetLabelById_ShouldReturnNull_WhenLabelDoesNotExist()
    {
        int labelId = 1;
        
        var result = await _repository.GetAsync(l => l.Id == labelId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetLabelsByBoardId_ShouldReturnLabels_WhenBoardHasLabels()
    {
        int boardId = 1;
        var label1 = new Label("title 1", "color", boardId) { Id = 1 };
        var label2 = new Label("title 2", "color", boardId) { Id = 2 };
        
        _context.Labels.AddRange(label1, label2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetListAsync(l => l.BoardId == boardId);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetLabelsByBoardId_ShouldReturnEmptyList_WhenBoardHasNoLabels()
    {
        int boardId = 1;
        
        var result = await _repository.GetListAsync(l => l.BoardId == boardId);
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddLabel_ShouldPersistLabel_WhenAddedSuccessfully()
    {
        var label = new Label("title", "color", boardId: 1) { Id = 1 };
        
        await _repository.CreateAsync(label);
        await _unitOfWork.CommitAsync();
        
        var result = await _context.Labels.FindAsync(label.Id);
        Assert.NotNull(result);
        Assert.Equal(label.Id, result.Id);
    }

    [Fact]
    public async Task UpdateLabel_ReturnsLabel_WhenLabelIsUpdatedSuccessfully()
    {
        var label = new Label("title", "color", boardId: 1) { Id = 1 };

        _context.Labels.Add(label);
        await _context.SaveChangesAsync();
        
        label.Title = "updated title";
        await _repository.UpdateAsync(label);
        await _unitOfWork.CommitAsync();
        
        var result = await _context.Labels.FindAsync(label.Id);
        Assert.NotNull(result);
        Assert.Equal("updated title", result.Title);
    }

    [Fact]
    public async Task DeleteLabel_ReturnsLabel_WhenLabelIsDeletedSuccessfully()
    {
        var label = new Label("title", "color", boardId: 1) { Id = 1 };

        _context.Labels.Add(label);
        await _context.SaveChangesAsync();
        
        await _repository.DeleteAsync(label);
        await _unitOfWork.CommitAsync();
        
        var result = await _context.Labels.FindAsync(label.Id);
        Assert.Null(result);
    }
}