using Microsoft.EntityFrameworkCore;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Data;
using TrelloApi.Infrastructure.Persistence.Interfaces;
using TrelloApi.Infrastructure.Persistence.Repositories;

namespace TrelloApi.Tests.Repositories;

public class CardLabelRepositoryTests
{
    private readonly CardLabelRepository _repository;
    private readonly TrelloContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public CardLabelRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TrelloContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TrelloContext(options);
        _unitOfWork = new FakeUnitOfWork(_context);
        _repository = new CardLabelRepository(_unitOfWork);
    }
    
    [Fact]
    public async Task GetCardLabelById_ShouldReturnCardLabel_WhenCardLabelExists()
    {
        int cardId = 1, labelId = 1;
        var cardLabel = new CardLabel(cardId, labelId);
        
        _context.CardLabels.Add(cardLabel);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetAsync(cl => cl.LabelId == labelId && cl.CardId == cardId);
        
        Assert.NotNull(result);
        Assert.Equal(cardLabel.CardId, result.CardId);
        Assert.Equal(cardLabel.LabelId, result.LabelId);
    }

    [Fact]
    public async Task GetCardLabelById_ShouldReturnNull_WhenCardLabelDoesNotExist()
    {
        int cardId = 1, labelId = 1;
        
        var result = await _repository.GetAsync(cl => cl.LabelId == labelId && cl.CardId == cardId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetLabelsByCardId_ShouldReturnLabels_WhenCardHasLabels()
    {
        int cardId = 1;
        var label1 = new Label("title", "color", boardId: 1) { Id = 1 };
        var label2 = new Label("title", "color", boardId: 1) { Id = 2 };
        var cardLabel1 = new CardLabel(cardId, label1.Id);
        var cardLabel2 = new CardLabel(cardId, label2.Id);

        _context.Labels.AddRange(label1, label2);
        _context.CardLabels.AddRange(cardLabel1, cardLabel2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetLabelsByCardIdAsync(cardId);
        
        Assert.NotNull(result);
        Assert.Equal(label1.Id, result.ElementAt(0).Id);
        Assert.Equal(label2.Id, result.ElementAt(1).Id);
    }

    [Fact]
    public async Task GetLabelsByCardId_ShouldReturnEmptyList_WhenCardHasNoLabels()
    {
        int cardId = 1;
        
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetLabelsByCardIdAsync(cardId);
        
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task AddCardLabel_ShouldPersistCardLabel_WhenAddedSuccessfully()
    {
        var cardLabel = new CardLabel(1, 1);
        
        await _repository.CreateAsync(cardLabel);
        await _unitOfWork.CommitAsync();
        
        var result = await _context.CardLabels.FindAsync(cardLabel.CardId, cardLabel.LabelId);
        
        Assert.NotNull(result);
        Assert.Equal(cardLabel.CardId, result.CardId);
        Assert.Equal(cardLabel.LabelId, result.LabelId);
    }

    [Fact]
    public async Task DeleteCardLabel_ShouldRemoveCardLabel_WhenCardLabelExists()
    {
        var cardLabel = new CardLabel(1, 1);

        _context.CardLabels.Add(cardLabel);
        await _context.SaveChangesAsync();
        
        await _repository.DeleteAsync(cardLabel);
        await _unitOfWork.CommitAsync();
        
        var result = await _context.CardLabels.FindAsync(cardLabel.CardId, cardLabel.LabelId);
        
        Assert.Null(result);
    }
}