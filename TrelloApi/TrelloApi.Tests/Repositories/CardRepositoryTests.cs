using Microsoft.EntityFrameworkCore;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Data;
using TrelloApi.Infrastructure.Persistence.Interfaces;
using TrelloApi.Infrastructure.Persistence.Repositories;

namespace TrelloApi.Tests.Repositories;

public class CardRepositoryTests
{
    private readonly ICardRepository _repository;
    private readonly TrelloContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public CardRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TrelloContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TrelloContext(options);
        _unitOfWork = new FakeUnitOfWork(_context);
        _repository = new CardRepository(_unitOfWork);
    }

    [Fact]
    public async Task GetCardById_ShouldReturnCard_WhenCardExists()
    {
        int cardId = 1;
        var card = new Card("title", "description", listId: 1) { Id = cardId };

        _context.Cards.Add(card);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAsync(c => c.Id == cardId);

        Assert.NotNull(result);
        Assert.Equal(cardId, result.Id);
    }

    [Fact]
    public async Task GetCardById_ShouldReturnNull_WhenCardDoesNotExist()
    {
        int cardId = 1;

        var result = await _repository.GetAsync(c => c.Id == cardId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCardsByListId_ShouldReturnCards_WhenListHasCards()
    {
        int listId = 1;
        var card1 = new Card("title1", "description1", listId) { Id = 1 };
        var card2 = new Card("title2", "description2", listId) { Id = 2 };

        _context.Cards.AddRange(card1, card2);
        await _context.SaveChangesAsync();

        var result = await _repository.GetListAsync(c => c.ListId == listId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetCardsByListId_ShouldReturnEmptyList_WhenListHasNoCards()
    {
        int listId = 1;

        var result = await _repository.GetListAsync(c => c.ListId == listId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task AddCard_ShouldPersistCard_WhenAddedSuccessfully()
    {
        var card = new Card("title", "description", listId: 1);

        await _repository.CreateAsync(card);
        await _unitOfWork.CommitAsync();

        var result = await _context.Cards.FindAsync(card.Id);

        Assert.NotNull(result);
        Assert.Equal(card.Id, result.Id);
    }

    [Fact]
    public async Task UpdateCard_ShouldPersistChanges_WhenUpdateIsSuccessful()
    {
        var card = new Card("title", "description", listId: 1);

        _context.Cards.Add(card);
        await _context.SaveChangesAsync();

        card.Title = "updated title";
        await _repository.UpdateAsync(card);
        await _unitOfWork.CommitAsync();

        var result = await _context.Cards.FindAsync(card.Id);

        Assert.NotNull(result);
        Assert.Equal("updated title", result.Title);
    }

    [Fact]
    public async Task DeleteCard_ShouldRemoveCard_WhenCardExists()
    {
        var card = new Card("title", "description", listId: 1);

        _context.Cards.Add(card);
        await _context.SaveChangesAsync();

        await _repository.DeleteAsync(card);
        await _unitOfWork.CommitAsync();

        var result = await _context.Cards.FindAsync(card.Id);

        Assert.Null(result);
    }
}