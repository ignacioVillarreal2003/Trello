using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.app;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence;
using TrelloApi.Infrastructure.Persistence.Data;
using TrelloApi.Infrastructure.Persistence.Repositories;

namespace TrelloApi.Tests.Repositories;

public class CardLabelRepositoryTests
{
    private readonly CardLabelRepository _repository;
    private readonly TrelloContext _context;
    private readonly Mock<ILogger<CardLabelRepository>> _mockLogger;

    public CardLabelRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TrelloContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TrelloContext(options);
        _mockLogger = new Mock<ILogger<CardLabelRepository>>();
        _repository = new CardLabelRepository(_context, _mockLogger.Object);
    }
    
    [Fact]
    public async Task GetCardLabelById_ShouldReturnCardLabel_WhenCardLabelExists()
    {
        int cardId = 1, labelId = 1;
        var cardLabel = new CardLabel(cardId: cardId, labelId: labelId);
        
        _context.CardLabels.Add(cardLabel);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetCardLabelById(cardId, labelId);
        
        Assert.NotNull(result);
        Assert.Equal(cardLabel.CardId, result.CardId);
        Assert.Equal(cardLabel.LabelId, result.LabelId);
    }

    [Fact]
    public async Task GetCardLabelById_ShouldReturnNull_WhenCardLabelDoesNotExist()
    {
        int cardId = 1, labelId = 1;
        
        var result = await _repository.GetCardLabelById(cardId, labelId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetLabelsByCardId_ShouldReturnLabels_WhenCardHasLabels()
    {
        int cardId = 1;
        var label1 = new Label(title: "title", color: "color", boardId: 1) { Id = 1 };
        var label2 = new Label(title: "title", color: "color", boardId: 1) { Id = 2 };
        var cardLabel1 = new CardLabel(cardId: cardId, labelId: label1.Id);
        var cardLabel2 = new CardLabel(cardId: cardId, labelId: label2.Id);

        _context.Labels.AddRange(label1, label2);
        _context.CardLabels.AddRange(cardLabel1, cardLabel2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetLabelsByCardId(cardId);
        
        Assert.NotNull(result);
        Assert.Equal(label1.Id, result[0].Id);
        Assert.Equal(label2.Id, result[1].Id);
    }

    [Fact]
    public async Task GetLabelsByCardId_ShouldReturnEmptyList_WhenCardHasNoLabels()
    {
        int cardId = 1;
        
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetLabelsByCardId(cardId);
        
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task AddCardLabel_ShouldPersistCardLabel_WhenAddedSuccessfully()
    {
        var cardLabel = new CardLabel(cardId: 1, labelId: 1);
        
        await _repository.AddCardLabel(cardLabel);
        var result = await _context.CardLabels.FindAsync(cardLabel.CardId, cardLabel.LabelId);
        
        Assert.NotNull(result);
        Assert.Equal(cardLabel.CardId, result.CardId);
        Assert.Equal(cardLabel.LabelId, result.LabelId);
    }

    [Fact]
    public async Task DeleteCardLabel_ShouldRemoveCardLabel_WhenCardLabelExists()
    {
        var cardLabel = new CardLabel(cardId: 1, labelId: 1);

        _context.CardLabels.Add(cardLabel);
        await _context.SaveChangesAsync();
        
        await _repository.DeleteCardLabel(cardLabel);
        var result = await _context.CardLabels.FindAsync(cardLabel.CardId, cardLabel.LabelId);
        
        Assert.Null(result);
    }
}