using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Services;
using TrelloApi.Domain.DTOs.Card;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Tests.Services;

public class CardServiceTests
{
    private readonly Mock<ICardRepository> _mockCardRepository;
    private readonly Mock<ILogger<CardService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CardService _service;

    public CardServiceTests()
    {
        _mockCardRepository = new Mock<ICardRepository>();
        _mockLogger = new Mock<ILogger<CardService>>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        
        _service = new CardService(
            _mockMapper.Object,
            _mockUnitOfWork.Object,
            _mockLogger.Object,
            _mockCardRepository.Object);
    }
    
    [Fact]
    public async Task GetCardById_ShouldReturnsCard_WhenCardFound()
    {
        const int cardId = 1;
        var card = new Card("title", "description", listId: 1, priority: "priority") { Id = cardId };
        var response = new CardResponse
            { Id = 1, Title = card.Title, Description = card.Description, Priority = card.Priority };
        
        _mockCardRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Card, bool>>>())).ReturnsAsync(card);
        _mockMapper.Setup(m => m.Map<CardResponse>(It.IsAny<Card>())).Returns(response);

        var result = await _service.GetCardById(cardId);

        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task GetCardById_ShouldReturnsNull_WhenCardNotFound()
    {
        const int cardId = 1;
        
        _mockCardRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Card, bool>>>())).ReturnsAsync((Card?)null);

        var result = await _service.GetCardById(cardId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCardsByListId_ShouldReturnsCards_WhenCardsFound()
    {
        const int listId = 1;
        var cards = new List<Card>
        {
            new Card("title 1", "description 1", listId, "priority") { Id = 1 },
            new Card("title 2", "description 2", listId, "priority") { Id = 2 }
        };
        var response = new List<CardResponse>
        {
            new CardResponse { Id = 1, Title = cards[0].Title, Description = cards[0].Description, Priority = cards[0].Priority },
            new CardResponse { Id = 2, Title = cards[1].Title, Description = cards[1].Description, Priority = cards[1].Priority }
        };
        
        _mockCardRepository.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<Card, bool>>>(), null)).ReturnsAsync(cards);
        _mockMapper.Setup(m => m.Map<List<CardResponse>>(It.IsAny<List<Card>>())).Returns(response);
        var result = await _service.GetCardsByListId(listId);

        Assert.NotNull(result);
        Assert.Equal(response.Count, result.Count);
    }

    [Fact]
    public async Task GetCardsByListId_ShouldReturnsEmptyList_WhenCardsNotFound()
    {
        const int listId = 1;

        _mockCardRepository.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<Card, bool>>>(), null)).ReturnsAsync([]);
        _mockMapper.Setup(m => m.Map<List<CardResponse>>(It.IsAny<List<Card>>())).Returns([]);

        var result = await _service.GetCardsByListId(listId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddCard_ShouldReturnsCard_WhenAddedSuccessful()
    {
        const int listId = 1;
        var dto = new AddCardDto { Title = "title", Description = "description", Priority = "priority" };
        var response = new CardResponse { Id = 1, Title = dto.Title, Description = dto.Description, Priority = dto.Priority };
        
        _mockCardRepository.Setup(r => r.CreateAsync(It.IsAny<Card>()));
        _mockMapper.Setup(m => m.Map<CardResponse>(It.IsAny<Card>())).Returns(response);
        
        var result = await _service.AddCard(listId, dto);

        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task UpdateCard_ShouldReturnsCard_WhenUpdatedSuccessful()
    {
        const int cardId = 1;
        var card = new Card("title", "description", listId: 1, priority: "priority") { Id = cardId };
        var dto = new UpdateCardDto { Title = "updated title" };
        var response = new CardResponse { Id = cardId, Title = dto.Title, Description = card.Description, Priority = card.Priority, ListId = card.ListId };

        _mockCardRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Card, bool>>>())).ReturnsAsync(card);
        _mockCardRepository.Setup(r => r.UpdateAsync(It.IsAny<Card>()));
        _mockMapper.Setup(m => m.Map<CardResponse>(It.IsAny<Card>())).Returns(response);

        var result = await _service.UpdateCard(cardId, dto);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateCard_ShouldReturnsNull_WhenUpdatedUnsuccessful()
    {
        const int cardId = 1;
        var dto = new UpdateCardDto { Title = "updated title" };

        _mockCardRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Card, bool>>>())).ReturnsAsync((Card?)null);
        
        var result = await _service.UpdateCard(cardId, dto);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task DeleteCard_ShouldReturnsTrue_WhenDeletedSuccessful()
    {
        const int cardId = 1;
        var card = new Card("title", "description", listId: 1, priority: "priority") { Id = cardId };

        _mockCardRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Card, bool>>>())).ReturnsAsync(card);
        _mockCardRepository.Setup(r => r.DeleteAsync(It.IsAny<Card>()));

        var result = await _service.DeleteCard(cardId);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteCard_ShouldReturnsFalse_WhenDeletedUnsuccessful()
    {
        const int cardId = 1;

        _mockCardRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Card, bool>>>())).ReturnsAsync((Card?)null);

        var result = await _service.DeleteCard(cardId);

        Assert.False(result);
    }
}
