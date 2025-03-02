using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Services;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.DTOs.CardLabel;
using TrelloApi.Domain.DTOs.Label;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Tests.Services;

public class CardLabelServiceTests
{
    private readonly Mock<ICardLabelRepository> _mockCardLabelRepository;
    private readonly Mock<ILogger<CardLabelService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CardLabelService _service;

    public CardLabelServiceTests()
    {
        _mockCardLabelRepository = new Mock<ICardLabelRepository>();
        _mockLogger = new Mock<ILogger<CardLabelService>>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        
        _service = new CardLabelService(
            _mockMapper.Object,
            _mockUnitOfWork.Object,
            _mockLogger.Object,
            _mockCardLabelRepository.Object);
    }

    [Fact]
    public async Task GetLabelsByCardId_ShouldReturnsLabels_WhenLabelsFound()
    {
        const int cardId = 1;
        var labels = new List<Label>
        {
            new Label("title", "color", 1),
            new Label("title", "color", 1)
        };
        var response = new List<LabelResponse>
        {
            new LabelResponse { Id = 0, Title = labels[0].Title, Color = labels[0].Color, BoardId = labels[0].BoardId},
            new LabelResponse { Id = 1, Title = labels[1].Title, Color = labels[1].Color, BoardId = labels[1].BoardId}
        };

        _mockCardLabelRepository.Setup(r => r.GetLabelsByCardIdAsync(cardId)).ReturnsAsync(labels);
        _mockMapper.Setup(m => m.Map<List<LabelResponse>>(It.IsAny<List<Label>>())).Returns(response);

        var result = await _service.GetLabelsByCardId(cardId);

        Assert.NotNull(result);
        Assert.Equal(response.Count, result.Count);
    }

    [Fact]
    public async Task GetLabelsByCardId_ShouldReturnsEmptyList_WhenLabelsNotFound()
    {
        const int cardId = 1;

        _mockCardLabelRepository.Setup(r => r.GetLabelsByCardIdAsync(cardId)).ReturnsAsync([]);
        _mockMapper.Setup(m => m.Map<List<LabelResponse>>(It.IsAny<List<Label>>())).Returns([]);

        var result = await _service.GetLabelsByCardId(cardId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddLabelToCard_ShouldReturnsCardLabel_WhenAddedSuccessful()
    {
        const int cardId = 1;
        var dto = new AddCardLabelDto { LabelId = 1 };
        var response = new CardLabelResponse{ CardId = cardId, LabelId = dto.LabelId };

        _mockCardLabelRepository.Setup(r => r.CreateAsync(It.IsAny<CardLabel>()));
        _mockMapper.Setup(m => m.Map<CardLabelResponse>(It.IsAny<CardLabel>())).Returns(response);

        var result = await _service.AddLabelToCard(cardId, dto);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task RemoveLabelFromCard_ShouldReturnsTrue_WhenDeletedSuccessful()
    {
        const int cardId = 1, labelId = 1;
        var cardLabel = new CardLabel(cardId, labelId);
        
        _mockCardLabelRepository.Setup(r => r
            .GetAsync(It.IsAny<Expression<Func<CardLabel, bool>>>())).ReturnsAsync(cardLabel);
        _mockCardLabelRepository.Setup(r => r.DeleteAsync(It.IsAny<CardLabel>()));

        var result = await _service.RemoveLabelFromCard(cardId, labelId);

        Assert.True(result);
    }

    [Fact]
    public async Task RemoveLabelFromCard_ShouldReturnsFalse_WhenDeletedUnsuccessful()
    {
        const int cardId = 1, labelId = 1;
        
        _mockCardLabelRepository.Setup(r => 
            r.GetAsync(It.IsAny<Expression<Func<CardLabel, bool>>>())).ReturnsAsync((CardLabel?)null);

        var result = await _service.RemoveLabelFromCard(cardId, labelId);

        Assert.False(result);
    }
}
