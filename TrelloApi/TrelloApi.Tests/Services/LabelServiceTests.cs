using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Services;
using TrelloApi.Domain.DTOs.Label;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Tests.Services;

public class LabelServiceTests
{
    private readonly Mock<ILabelRepository> _mockLabelRepository;
    private readonly Mock<ILogger<LabelService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly LabelService _service;

    public LabelServiceTests()
    {
        _mockLabelRepository = new Mock<ILabelRepository>();
        _mockLogger = new Mock<ILogger<LabelService>>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _service = new LabelService(
            _mockMapper.Object,
            _mockUnitOfWork.Object,
            _mockLogger.Object,
            _mockLabelRepository.Object);
    }
    
    [Fact]
    public async Task GetLabelById_ShouldReturnsLabel_WhenLabelFound()
    {
        const int labelId = 1;
        var label = new Label(title: "title", color: "color", boardId: 1) { Id = labelId };
        var response = new LabelResponse { Id = label.Id, Title = label.Title, Color = label.Color };

        _mockLabelRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Label, bool>>>())).ReturnsAsync(label);
        _mockMapper.Setup(m => m.Map<LabelResponse>(label)).Returns(response);

        var result = await _service.GetLabelById(labelId);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetLabelById_ShouldReturnsNull_WhenLabelNotFound()
    {
        const int labelId = 1;
        
        _mockLabelRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Label, bool>>>())).ReturnsAsync((Label?)null);

        var result = await _service.GetLabelById(labelId);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetLabelsByBoardId_ShouldReturnsLabels_WhenLabelsFound()
    {
        const int boardId = 1;
        var labels = new List<Label>
        {
            new Label(title: "title 1", color: "color", boardId: boardId) { Id = 1 },
            new Label(title: "title 2", color: "color", boardId: boardId) { Id = 2 }
        };
        var response = new List<LabelResponse>
        {
            new LabelResponse { Id = labels[0].Id, Title = labels[0].Title, Color = labels[0].Color },
            new LabelResponse { Id = labels[1].Id, Title = labels[1].Title, Color = labels[1].Color }
        };

        _mockLabelRepository.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<Label, bool>>>(), null)).ReturnsAsync(labels);
        _mockMapper.Setup(m => m.Map<List<LabelResponse>>(It.IsAny<List<Label>>())).Returns(response);

        var result = await _service.GetLabelsByBoardId(boardId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetLabelsByBoardId_ShouldReturnsEmptyList_WhenLabelsNotFound()
    {
        const int boardId = 1;
        
        _mockLabelRepository.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<Label, bool>>>(), null)).ReturnsAsync([]);
        _mockMapper.Setup(m => m.Map<List<LabelResponse>>(It.IsAny<List<Label>>())).Returns([]);

        var result = await _service.GetLabelsByBoardId(boardId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task AddLabel_ShouldReturnsLabel_WhenAddedSuccessfully()
    {
        const int boardId = 1;
        var dto = new AddLabelDto { Title = "title", Color = "color" };
        var label = new Label(title: dto.Title, color: dto.Color, boardId: boardId) { Id = 1 };
        var response = new LabelResponse { Id = label.Id, Title = label.Title, Color = label.Color };

        _mockLabelRepository.Setup(r => r.CreateAsync(It.IsAny<Label>())).ReturnsAsync(label);
        _mockMapper.Setup(m => m.Map<LabelResponse>(It.IsAny<Label>())).Returns(response);

        var result = await _service.AddLabel(boardId, dto);

        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task UpdateLabel_ShouldReturnsLabel_WhenUpdatedSuccessful()
    {
        const int labelId = 1;
        var label = new Label(title: "title", color: "color", boardId: 1) { Id = labelId };
        var dto = new UpdateLabelDto { Title = "updated title" };
        var response = new LabelResponse { Id = label.Id, Title = dto.Title, Color = label.Color, BoardId = label.BoardId };
        
        _mockLabelRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Label, bool>>>())).ReturnsAsync(label);
        _mockLabelRepository.Setup(r => r.UpdateAsync(It.IsAny<Label>()));
        _mockMapper.Setup(m => m.Map<LabelResponse>(It.IsAny<Label>())).Returns(response);
        
        var result = await _service.UpdateLabel(labelId, dto);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateLabel_ShouldReturnsNull_WhenUpdatedUnsuccessful()
    {
        const int labelId = 1;
        var dto = new UpdateLabelDto { Title = "title", Color = "color" };

        _mockLabelRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Label, bool>>>())).ReturnsAsync((Label?)null);

        var result = await _service.UpdateLabel(labelId, dto);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task DeleteLabel_ShouldReturnsTrue_WhenDeletedSuccessful()
    {
        const int labelId = 1;
        var label = new Label(title: "title", color: "color", boardId: 1) { Id = labelId };

        _mockLabelRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Label, bool>>>())).ReturnsAsync(label);
        _mockLabelRepository.Setup(r => r.DeleteAsync(It.IsAny<Label>()));

        var result = await _service.DeleteLabel(labelId);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteLabel_ShouldReturnsFalse_WhenDeletedUnsuccessful()
    {
        const int labelId = 1;
        
        _mockLabelRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Label, bool>>>())).ReturnsAsync((Label?)null);

        var result = await _service.DeleteLabel(labelId);

        Assert.False(result);
    }
}
