using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Services;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;
using TrelloApi.Domain.Label;
using Task = System.Threading.Tasks.Task;

namespace TrelloApi.Tests.Services;

public class LabelServiceTests
{
    private readonly Mock<ILabelRepository> _mockLabelRepository;
    private readonly Mock<ILogger<LabelService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IBoardAuthorizationService> _mockBoardAuthorizationService;
    private readonly LabelService _service;

    public LabelServiceTests()
    {
        _mockLabelRepository = new Mock<ILabelRepository>();
        _mockLogger = new Mock<ILogger<LabelService>>();
        _mockMapper = new Mock<IMapper>();
        _mockBoardAuthorizationService = new Mock<IBoardAuthorizationService>();

        _service = new LabelService(_mockMapper.Object, _mockBoardAuthorizationService.Object, _mockLogger.Object, _mockLabelRepository.Object);
    }
    
    [Fact]
    public async Task GetLabelById_ReturnsOutputLabelDto_WhenLabelExists()
    {
        int labelId = 1, userId = 1;
        var label = new Label(title: "Label 1", color: "Blue", boardId: 1) { Id = labelId };
        var outputLabelDto = new OutputLabelDto { Id = label.Id, Title = label.Title, Color = label.Color };

        _mockLabelRepository.Setup(r => r.GetLabelById(labelId)).ReturnsAsync(label);
        _mockMapper.Setup(m => m.Map<OutputLabelDto>(label)).Returns(outputLabelDto);

        var result = await _service.GetLabelById(labelId, userId);

        Assert.NotNull(result);
        Assert.Equal(outputLabelDto.Id, result.Id);
        Assert.Equal(outputLabelDto.Title, result.Title);
        Assert.Equal(outputLabelDto.Color, result.Color);
    }

    [Fact]
    public async Task GetLabelById_ReturnsNull_WhenLabelDoesNotExist()
    {
        int labelId = 1, userId = 1;
        
        _mockLabelRepository.Setup(r => r.GetLabelById(labelId)).ReturnsAsync((Label?)null);

        var result = await _service.GetLabelById(labelId, userId);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetLabelsByTaskId_ReturnsListOfOutputLabelDto_WhenLabelsExist()
    {
        int taskId = 1, userId = 1;
        var labels = new List<Label>
        {
            new Label(title: "Label 1", color: "Blue", boardId: 1) { Id = 1 },
            new Label(title: "Label 2", color: "Blue", boardId: 1) { Id = 2 }
        };
        var outputLabelDtos = new List<OutputLabelDto>
        {
            new OutputLabelDto { Id = labels[0].Id, Title = labels[0].Title, Color = labels[0].Color },
            new OutputLabelDto { Id = labels[1].Id, Title = labels[1].Title, Color = labels[1].Color }
        };

        _mockLabelRepository.Setup(r => r.GetLabelsByTaskId(taskId)).ReturnsAsync(labels);
        _mockMapper.Setup(m => m.Map<List<OutputLabelDto>>(labels)).Returns(outputLabelDtos);

        var result = await _service.GetLabelsByTaskId(taskId, userId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(outputLabelDtos[0].Id, result[0].Id);
        Assert.Equal(outputLabelDtos[1].Id, result[1].Id);
    }

    [Fact]
    public async Task GetLabelsByTaskId_ReturnsEmptyLabel_WhenNoLabelsExist()
    {
        int taskId = 1, userId = 1;

        _mockLabelRepository.Setup(r => r.GetLabelsByTaskId(taskId)).ReturnsAsync(new List<Label>());
        _mockMapper.Setup(m => m.Map<List<OutputLabelDto>>(It.IsAny<List<Label>>())).Returns(new List<OutputLabelDto>());

        var result = await _service.GetLabelsByTaskId(taskId, userId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task AddLabel_ReturnsOutputLabelDto_WhenLabelIsAdded()
    {
        int taskId = 1, userId = 1;
        var addLabelDto = new AddLabelDto { Title = "New Label", Color = "Blue" };
        var newLabel = new Label(title: addLabelDto.Title, color: addLabelDto.Color, boardId: 1) { Id = 1 };
        var outputLabelDto = new OutputLabelDto { Id = newLabel.Id, Title = newLabel.Title, Color = newLabel.Color };

        _mockLabelRepository.Setup(r => r.AddLabel(It.IsAny<Label>())).ReturnsAsync(newLabel);
        _mockMapper.Setup(m => m.Map<OutputLabelDto>(newLabel)).Returns(outputLabelDto);

        var result = await _service.AddLabel(taskId, addLabelDto, userId);

        Assert.NotNull(result);
        Assert.Equal(outputLabelDto.Id, result.Id);
        Assert.Equal(outputLabelDto.Title, result.Title);
        Assert.Equal(outputLabelDto.Color, result.Color);
    }

    [Fact]
    public async Task AddLabel_ReturnsNull_WhenRepositoryReturnsNull()
    {
        int taskId = 1, userId = 1;
        var addLabelDto = new AddLabelDto { Title = "New Label", Color = "Blue" };

        _mockLabelRepository.Setup(r => r.AddLabel(It.IsAny<Label>())).ReturnsAsync((Label?)null);

        var result = await _service.AddLabel(taskId, addLabelDto, userId);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task UpdateLabel_ReturnsOutputLabelDto_WhenUpdateIsSuccessful()
    {
        int labelId = 1, userId = 1;
        var existingLabel = new Label(title: "Old Title", color: "Blue", boardId: 1) { Id = 1 };
        var updateLabelDto = new UpdateLabelDto { Title = "New Title" };
        var updatedLabel = new Label(title: updateLabelDto.Title, color: existingLabel.Color, boardId: existingLabel.BoardId) { Id = existingLabel.Id };
        var outputLabelDto = new OutputLabelDto { Id = updatedLabel.Id, Title = updatedLabel.Title, Color = updatedLabel.Color };

        _mockLabelRepository.Setup(r => r.GetLabelById(labelId)).ReturnsAsync(existingLabel);
        _mockLabelRepository.Setup(r => r.UpdateLabel(existingLabel)).ReturnsAsync(updatedLabel);
        _mockMapper.Setup(m => m.Map<OutputLabelDto>(updatedLabel)).Returns(outputLabelDto);

        var result = await _service.UpdateLabel(labelId, updateLabelDto, userId);

        Assert.NotNull(result);
        Assert.Equal(outputLabelDto.Id, result.Id);
        Assert.Equal(outputLabelDto.Title, result.Title);
        Assert.Equal(outputLabelDto.Color, result.Color);
    }

    [Fact]
    public async Task UpdateLabel_ReturnsNull_WhenLabelNotFound()
    {
        int labelId = 1, userId = 1;
        var updateLabelDto = new UpdateLabelDto { Title = "New Title" };

        _mockLabelRepository.Setup(r => r.GetLabelById(labelId)).ReturnsAsync((Label?)null);

        var result = await _service.UpdateLabel(labelId, updateLabelDto, userId);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateLabel_ReturnsNull_WhenUpdateFails()
    {
        int labelId = 1, userId = 1;
        var existingLabel = new Label(title: "Old Title", color: "Blue", boardId: 1) { Id = 1 };
        var updateLabelDto = new UpdateLabelDto { Title = "New Title" };

        _mockLabelRepository.Setup(r => r.GetLabelById(labelId)).ReturnsAsync(existingLabel);
        _mockLabelRepository.Setup(r => r.UpdateLabel(existingLabel)).ReturnsAsync((Label?)null);

        var result = await _service.UpdateLabel(labelId, updateLabelDto, userId);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task DeleteLabel_ReturnsOutputLabelDto_WhenDeletionIsSuccessful()
    {
        int labelId = 1, userId = 1;
        var existingLabel = new Label(title: "Old Title", color: "Blue", boardId: 1) { Id = 1 };
        var deletedLabel = existingLabel;
        var outputLabelDto = new OutputLabelDto { Id = deletedLabel.Id, Title = deletedLabel.Title, Color = deletedLabel.Color };

        _mockLabelRepository.Setup(r => r.GetLabelById(labelId)).ReturnsAsync(existingLabel);
        _mockLabelRepository.Setup(r => r.DeleteLabel(existingLabel)).ReturnsAsync(deletedLabel);
        _mockMapper.Setup(m => m.Map<OutputLabelDto>(deletedLabel)).Returns(outputLabelDto);

        var result = await _service.DeleteLabel(labelId, userId);

        Assert.NotNull(result);
        Assert.Equal(outputLabelDto.Id, result.Id);
        Assert.Equal(outputLabelDto.Title, result.Title);
        Assert.Equal(outputLabelDto.Color, result.Color);
    }

    [Fact]
    public async Task DeleteLabel_ReturnsNull_WhenLabelNotFound()
    {
        int labelId = 1, userId = 1;
        
        _mockLabelRepository.Setup(r => r.GetLabelById(labelId)).ReturnsAsync((Label?)null);

        var result = await _service.DeleteLabel(labelId, userId);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteLabel_ReturnsNull_WhenDeletionFails()
    {
        int labelId = 1, userId = 1;
        var existingLabel = new Label(title: "Old Title", color: "Blue", boardId: 1) { Id = 1 };

        _mockLabelRepository.Setup(r => r.GetLabelById(labelId)).ReturnsAsync(existingLabel);
        _mockLabelRepository.Setup(r => r.DeleteLabel(existingLabel)).ReturnsAsync((Label?)null);

        var result = await _service.DeleteLabel(labelId, userId);

        Assert.Null(result);
    }
}