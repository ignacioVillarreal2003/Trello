using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.app;
using TrelloApi.Domain.Entities;
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
    public async Task GetCardById_ShouldReturnCard_WhenCardExists()
    {
        int cardId = 1;
        var card = new Card(title: "title", description: "description", listId: 1) { Id = cardId };

        _context.Cards.Add(card);
        await _context.SaveChangesAsync();

        var result = await _repository.GetCardById(cardId);

        Assert.NotNull(result);
        Assert.Equal(cardId, result.Id);
    }

    [Fact]
    public async Task GetCardById_ShouldReturnNull_WhenCardDoesNotExist()
    {
        int cardId = 1;

        var result = await _repository.GetCardById(cardId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCardsByListId_ShouldReturnCards_WhenListHasCards()
    {
        int listId = 1;
        var card1 = new Card(title: "title1", description: "description1", listId: listId) { Id = 1 };
        var card2 = new Card(title: "title2", description: "description2", listId: listId) { Id = 2 };

        _context.Cards.AddRange(card1, card2);
        await _context.SaveChangesAsync();

        var result = await _repository.GetCardsByListId(listId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetCardsByListId_ShouldReturnEmptyList_WhenListHasNoCards()
    {
        int listId = 1;

        var result = await _repository.GetCardsByListId(listId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task AddCard_ShouldPersistCard_WhenAddedSuccessfully()
    {
        var card = new Card(title: "title", description: "description", listId: 1);

        await _repository.AddCard(card);
        var result = await _context.Cards.FindAsync(card.Id);

        Assert.NotNull(result);
        Assert.Equal(card.Id, result.Id);
    }

    [Fact]
    public async Task UpdateCard_ShouldPersistChanges_WhenUpdateIsSuccessful()
    {
        var card = new Card(title: "title", description: "description", listId: 1);

        _context.Cards.Add(card);
        await _context.SaveChangesAsync();

        card.Title = "updated title";
        await _repository.UpdateCard(card);
        var result = await _context.Cards.FindAsync(card.Id);

        Assert.NotNull(result);
        Assert.Equal("updated title", result.Title);
    }

    [Fact]
    public async Task DeleteCard_ShouldRemoveCard_WhenCardExists()
    {
        var card = new Card(title: "title", description: "description", listId: 1);

        _context.Cards.Add(card);
        await _context.SaveChangesAsync();

        await _repository.DeleteCard(card);

        var result = await _context.Cards.FindAsync(card.Id);

        Assert.Null(result);
    }
}
